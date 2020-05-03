using System;

namespace SqlCrawler.Backend.Sqlite
{
    public class SessionRecord
    {
        public long RowId { get; set; }
        public string QueryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime FinishedAtUtc { get; set; }
    }
}