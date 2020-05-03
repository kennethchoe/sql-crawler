using System;

namespace SqlCrawler.Backend.Sqlite
{
    public class ResultRecord
    {
        public long SessionId { get; set; }
        public string ServerId { get; set; }
        public string QueryName { get; set; }
        public string DataJson { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}