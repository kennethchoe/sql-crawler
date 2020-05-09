using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SqlCrawler.Backend.Persistence
{
    public class ResultRecord
    {
        public long SessionRowId { get; set; }
        public string QueryName { get; set; }
        public string ServerId { get; set; }
        public string DataJson { get; set; }

        public IEnumerable<dynamic> Data => JsonConvert.DeserializeObject<IEnumerable<dynamic>>(DataJson);

        public DateTime CreatedAtUtc { get; set; }
    }
}