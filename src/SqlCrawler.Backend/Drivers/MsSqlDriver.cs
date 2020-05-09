using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend.Drivers
{
    public class MsSqlDriver: IServerDriver
    {
        private readonly IAppConfig _config;

        public MsSqlDriver(IAppConfig config)
        {
            _config = config;
        }

        public bool CanHandle(string driverName)
        {
            return driverName == "mssql";
        }

        public string BuildConnectionString(SqlServerInfo serverInfo)
        {
            var result = "Data Source=" + serverInfo.DataSource + ";";

            if (serverInfo.UseIntegratedSecurity)
                result += "Integrated Security=SSPI;";
            else
                result += $"User Id={serverInfo.SqlUsername};Password={serverInfo.SqlPassword};";

            if (_config.ConnectionTimeoutInSeconds != null)
                result += "Connect Timeout=" + _config.ConnectionTimeoutInSeconds + ";";

            return result;
        }

        public async Task<IEnumerable<dynamic>> Run(string connectionString, string sql, SqlServerInfoPublic serverInfo,
            CancellationToken cancellationToken)
        {
            var conn = new SqlConnection(connectionString);
            var data = await conn.QueryAsync(
                new CommandDefinition(sql, serverInfo, cancellationToken: cancellationToken,
                    commandTimeout: _config.CommandTimeoutInSeconds));
            return data;
        }
    }
}
