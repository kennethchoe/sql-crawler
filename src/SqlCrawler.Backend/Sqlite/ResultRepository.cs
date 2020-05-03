using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;

namespace SqlCrawler.Backend.Sqlite
{
    public class ResultRepository
    {
        private readonly SqliteConnection _conn;

        public ResultRepository(SqliteConnection conn)
        {
            _conn = conn;
        }

        public void Insert(ResultRecord record)
        {
            record.CreatedAtUtc = DateTime.UtcNow;
            
            _conn.Execute(
                "insert into Results(SessionId, ServerId, QueryName, Data, CreatedAtUtc) values(@SessionId, @ServerId, @QueryName, @DataJson, @CreatedAt)",
                record);
        }

        public IEnumerable<ResultRecord> Get(string queryName, string serverId)
        {
            var result = _conn.Query<ResultRecord>(@"
select a.* 
  from Results a 
       inner join Sessions b on a.SessionId = b.SessionId and b.IsActive = 1
 where a.QueryName = coalesce(@queryName, a.QueryName)
   and a.ServerId = coalesce(@serverId, a.ServerId)
", new { queryName, serverId });
            return result;
        }
    }
}
