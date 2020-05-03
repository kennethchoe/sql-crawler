using System;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace SqlCrawler.Backend.Sqlite
{
    public class SessionRepository
    {
        private readonly SqliteConnection _conn;

        public SessionRepository(SqliteConnection conn)
        {
            _conn = conn;
        }

        public SessionRecord Insert(SessionRecord record)
        {
            record.StartedAtUtc = DateTime.UtcNow;

            var id = _conn.Query<int>(
                "insert into Sessions(QueryName, StartedAtUtc) values(@QueryName, @DataJson, StartedAtUtc); select @@identity()",
                record).Single();

            record.Id = id;

            return record;
        }

        public void Finish(SessionRecord sessionRecord)
        {
            sessionRecord.FinishedAtUtc = DateTime.UtcNow;

            _conn.Execute(@"
update Sessions set IsActive = 1, FinishedAtUtc = @FinishedAtUtc where Id = @Id;
update Sessions set IsActive = 0 where Id <> @Id and IsActive = 1;
", sessionRecord);
        }
    }
}