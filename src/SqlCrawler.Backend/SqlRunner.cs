using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HandlebarsDotNet;
using LibGit2Sharp;
using Newtonsoft.Json;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Sqlite;

namespace SqlCrawler.Backend
{
    public class SqlRunner
    {
        private readonly SqlCredentialReader _credentialReader;
        private readonly SqlSourceReader _sourceReader;
        private readonly IAppConfig _appConfig;
        private readonly SessionRepository _sessionRepository;
        private readonly ResultRepository _resultRepository;

        public SqlRunner(SqlCredentialReader credentialReader, SqlSourceReader sourceReader,
            IAppConfig appConfig,
            SessionRepository sessionRepository,
            ResultRepository resultRepository)
        {
            _credentialReader = credentialReader;
            _sourceReader = sourceReader;
            _appConfig = appConfig;
            _sessionRepository = sessionRepository;
            _resultRepository = resultRepository;
        }

        public async Task Run(string queryName, CancellationToken cancellationToken)
        {
            var sqls = _sourceReader.Read();
            var template = Handlebars.Compile(sqls.Single(x => x.Name == queryName).Query);

            var servers = _credentialReader.Read();

            var sessionRecord = _sessionRepository.Insert(new SessionRecord
            {
                QueryName = queryName
            });

            foreach (var server in servers)
            {
                if (cancellationToken.IsCancellationRequested) throw new UserCancelledException();

                var processed = template(server);

                var conn = new SqlConnection(server.ToConnectionString());
                var data = await conn.QueryAsync(
                    new CommandDefinition(processed, server, cancellationToken: cancellationToken, commandTimeout: _appConfig.CommandTimeoutInSeconds));

                _resultRepository.Insert(new ResultRecord
                {
                    SessionRowId = sessionRecord.RowId,
                    ServerId = server.ServerId,
                    DataJson = JsonConvert.SerializeObject(data)
                });
            }

            _sessionRepository.Finish(sessionRecord);
        }
    }
}
