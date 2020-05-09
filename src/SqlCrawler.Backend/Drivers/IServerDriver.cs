using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend.Drivers
{
    public interface IServerDriver
    {
        bool CanHandle(string driverName);
        string BuildConnectionString(SqlServerInfo serverInfo);
        Task<IEnumerable<dynamic>> Run(string connectionString, string sql, SqlServerInfoPublic serverInfo,
            CancellationToken cancellationToken);
    }
}