using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend
{
    public class SqlCredentialReader
    {
        private readonly IAppConfig _appConfig;

        public SqlCredentialReader(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        public IEnumerable<SqlServerInfo> Read()
        {
            using var reader = new StreamReader(Path.Combine(_appConfig.SqlCredentialsFilePath));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<SqlServerInfo>().ToList();

            var dups = records.GroupBy(x => x.ServerId).Where(x => x.Count() > 1);
            if (dups.Any())
                throw new SqlCredentialsException("ServerId must be unique. Duplicated Id(s): " + 
                                    dups.Select(x => x.Key).Aggregate((x, y) => x + ", " + y));
            
            return records;
        }
    }
}
