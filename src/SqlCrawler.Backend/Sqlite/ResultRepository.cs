using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;
using SqlCrawler.Backend.Core;

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
                "insert into Results(SessionRowId, ServerId, DataJson, CreatedAtUtc) values(@SessionRowId, @ServerId, @DataJson, @CreatedAtUtc)",
                record);
        }

        public IEnumerable<ResultRecord> Get(string queryName, string serverId)
        {
            var result = _conn.Query<ResultRecord>(@"
select a.* 
  from Results a 
       inner join Sessions b on a.SessionRowId = b.RowId and b.IsActive = 1
 where b.QueryName = coalesce(@queryName, b.QueryName)
   and a.ServerId = coalesce(@serverId, a.ServerId)
", new { queryName, serverId });
            return result;
        }
    }
}
