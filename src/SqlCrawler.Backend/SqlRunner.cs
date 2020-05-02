using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HandlebarsDotNet;
using LibGit2Sharp;

namespace SqlCrawler.Backend
{
    public class SqlRunner
    {
        private readonly SqlCredentialReader _credentialReader;
        private readonly SqlSourceReader _sourceReader;
        private readonly IAppConfig _appConfig;

        public SqlRunner(SqlCredentialReader credentialReader, SqlSourceReader sourceReader,
            IAppConfig appConfig)
        {
            _credentialReader = credentialReader;
            _sourceReader = sourceReader;
            _appConfig = appConfig;
        }

        public async Task<IEnumerable<RunResult>> Run(string sqlKey, CancellationToken cancellationToken)
        {
            var sqls = _sourceReader.Read();
            var template = Handlebars.Compile(sqls[sqlKey]);

            var servers = _credentialReader.Read();

            var result = new List<RunResult>();
            foreach (var server in servers)
            {
                if (cancellationToken.IsCancellationRequested) throw new UserCancelledException();

                var processed = template(server);
                var runResult = new RunResult
                {
                    ServerInfo = server,
                    QueryName = sqlKey,
                };

                var conn = new SqlConnection(server.ToConnectionString());
                runResult.Result = await conn.QueryAsync(
                    new CommandDefinition(processed, server, cancellationToken: cancellationToken, commandTimeout: _appConfig.CommandTimeoutInSeconds));

                result.Add(runResult);
            }

            return result;
        }
    }

    public class RunResult
    {
        public SqlServerInfo ServerInfo { get; set; }
        public string QueryName { get; set; }
        public IEnumerable<dynamic> Result { get; set; }
    }
}
