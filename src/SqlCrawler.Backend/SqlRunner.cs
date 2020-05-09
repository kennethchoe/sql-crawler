using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using LibGit2Sharp;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Serilog;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Drivers;
using SqlCrawler.Backend.Persistence;

namespace SqlCrawler.Backend
{
    public class SqlRunner
    {
        private readonly SqlCredentialReader _credentialReader;
        private readonly SqlQueryReader _queryReader;
        private readonly SessionRepository _sessionRepository;
        private readonly ResultRepository _resultRepository;
        private readonly IEnumerable<IServerDriver> _drivers;

        public SqlRunner(SqlCredentialReader credentialReader, SqlQueryReader queryReader,
            SessionRepository sessionRepository,
            ResultRepository resultRepository,
            IEnumerable<IServerDriver> drivers)
        {
            _credentialReader = credentialReader;
            _queryReader = queryReader;
            _sessionRepository = sessionRepository;
            _resultRepository = resultRepository;
            _drivers = drivers;
        }

        public async Task Run(string queryName, CancellationToken cancellationToken, Action<int> init, Action progress)
        {
            var sqls = _queryReader.Read();
            var sql = sqls.Single(x => x.Name == queryName);
            var template = Handlebars.Compile(sql.Query);

            var servers = _credentialReader.Read();
            var serversFiltered = servers.Where(x => string.IsNullOrEmpty(sql.Scope) || x.Scope == sql.Scope || x.Scope.StartsWith(sql.Scope + "/")).ToList();
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
                    var driver = GetDriver(server);
                    var connectionString = driver.BuildConnectionString(server);
                    var data = await driver.Run(connectionString, processed, serverPublic, cancellationToken);
                    dataJson = JsonConvert.SerializeObject(data);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error while polling from server " + server.ServerId);
                    dataJson = "[{Error: " + JsonConvert.ToString(e.Message) + "}]";
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

        public IServerDriver GetDriver(SqlServerInfo server)
        {
            var driver = _drivers.SingleOrDefault(x => x.CanHandle(server.ServerDriver));
            if (driver == null) throw new Exception("Unknown ServerDriver: " + server.ServerDriver);
            return driver;
        }
    }
}
