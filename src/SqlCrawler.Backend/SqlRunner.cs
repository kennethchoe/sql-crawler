using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HandlebarsDotNet;
using LibGit2Sharp;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Serilog;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Sqlite;

namespace SqlCrawler.Backend
{
    public class SqlRunner
    {
        private readonly SqlCredentialReader _credentialReader;
        private readonly SqlQueryReader _queryReader;
        private readonly IAppConfig _appConfig;
        private readonly SessionRepository _sessionRepository;
        private readonly ResultRepository _resultRepository;

        public SqlRunner(SqlCredentialReader credentialReader, SqlQueryReader queryReader,
            IAppConfig appConfig,
            SessionRepository sessionRepository,
            ResultRepository resultRepository)
        {
            _credentialReader = credentialReader;
            _queryReader = queryReader;
            _appConfig = appConfig;
            _sessionRepository = sessionRepository;
            _resultRepository = resultRepository;
        }

        public async Task Run(string queryName, CancellationToken cancellationToken, Action<int> init, Action progress)
        {
            var sqls = _queryReader.Read();
            var sql = sqls.Single(x => x.Name == queryName);
            var template = Handlebars.Compile(sql.Query);

            var servers = _credentialReader.Read();
            var serversFiltered = servers.Where(x => x.Scope.StartsWith(sql.Scope)).ToList();
            init(serversFiltered.Count);

            var sessionRecord = _sessionRepository.Insert(new SessionRecord
            {
                QueryName = queryName
            });

            foreach (var server in serversFiltered)
            {
                var serverPublic = new SqlServerInfoPublic();
                serverPublic.InjectFrom(server);

                if (cancellationToken.IsCancellationRequested) throw new UserCancelledException();

                var processed = template(serverPublic);

                string dataJson;
                try
                {
                    var conn = new SqlConnection(server.ToConnectionString(_appConfig.ConnectionTimeoutInSeconds));
                    var data = await conn.QueryAsync(
                        new CommandDefinition(processed, serverPublic, cancellationToken: cancellationToken,
                            commandTimeout: _appConfig.CommandTimeoutInSeconds));
                    dataJson = JsonConvert.SerializeObject(data);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error while polling from server " + server.ServerId);
                    dataJson = "[{Error: '" + e.Message.Replace("'", "\\'") + "'}]";
                }

                _resultRepository.Insert(new ResultRecord
                {
                    SessionRowId = sessionRecord.RowId,
                    ServerId = server.ServerId,
                    DataJson = dataJson
                });

                progress();
            }

            _sessionRepository.Finish(sessionRecord);
        }
    }
}
