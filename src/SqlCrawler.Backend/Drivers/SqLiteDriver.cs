using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend.Drivers
{
    public class SqLiteDriver : IServerDriver
    {
        private readonly IAppConfig _config;

        public SqLiteDriver(IAppConfig config)
        {
            _config = config;
        }

        public bool CanHandle(string driverName)
        {
            return driverName == "sqlite";
        }

        public string BuildConnectionString(SqlServerInfo serverInfo)
        {
            var result = "Data Source=" + serverInfo.DataSource + ";";

            if (!string.IsNullOrEmpty(serverInfo.SqlPassword))
                result += "Password=" + serverInfo.SqlPassword + ";";

            return result;
        }

        public async Task<IEnumerable<dynamic>> Run(string connectionString, string sql, SqlServerInfoPublic serverInfo,
            CancellationToken cancellationToken)
        {
            var conn = new SqliteConnection(connectionString);
            var data = await conn.QueryAsync(
                new CommandDefinition(sql, serverInfo, cancellationToken: cancellationToken,
                    commandTimeout: _config.CommandTimeoutInSeconds));
            return data;
        }
    }
}
