using System;

namespace SqlCrawler.Backend.Core
{
    public class SqlQueryInfo
    {
        public string Scope { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public DateTime? LastRetrievedAtUtc { get; set; }
    }
}