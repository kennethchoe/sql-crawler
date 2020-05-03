using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlCrawler.Backend.Sqlite;

namespace SqlCrawler.Backend
{
    public class Tabularizer
    {
        public IEnumerable<dynamic> Process(IEnumerable<ResultRecord> records)
        {
            foreach (var record in records)
            {
                dynamic result = new ExpandoObject();
                var kv = (IDictionary<string, object>)result;
                IEnumerable<dynamic> deserialized = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(record.DataJson).ToList();

                if (deserialized.Count() == 1)
                {
                    Newtonsoft.Json.Linq.JContainer props = deserialized.Single();
                    foreach (var jToken in props)
                    {
                        var prop = (JProperty) jToken;
                        kv.Add(prop.Path, prop.Value);
                    };
                }
                else if (deserialized.Any())
                {
                    result.Rows = deserialized;
                }

                result.ServerId = record.ServerId;
                yield return result;
            }
        }
    }
}