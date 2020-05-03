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

            var rowId = _conn.Query<int>(
                "insert into Sessions(QueryName, IsActive, StartedAtUtc) values(@QueryName, 0, @StartedAtUtc); select last_insert_rowid()",
                record).Single();

            record.RowId = rowId;

            return record;
        }

        public void Finish(SessionRecord sessionRecord)
        {
            sessionRecord.FinishedAtUtc = DateTime.UtcNow;

            _conn.Execute(@"
update Sessions set IsActive = 1, FinishedAtUtc = @FinishedAtUtc where RowId = @RowId;
update Sessions set IsActive = 0 where RowId <> @RowId and IsActive = 1;
", sessionRecord);
        }
    }
}