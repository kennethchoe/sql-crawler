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
            csv.Configuration.RegisterClassMap(new SqlServerInfoMap());

            var records = csv.GetRecords<SqlServerInfo>().ToList();

            var duplicates = records.GroupBy(x => x.ServerId).Where(x => x.Count() > 1).ToList();
            if (duplicates.Any())
                throw new SqlCredentialsException("ServerId must be unique. Duplicated Id(s): " + 
                                    duplicates.Select(x => x.Key).Aggregate((x, y) => x + ", " + y));
            
            return records;
        }

        private sealed class SqlServerInfoMap : ClassMap<SqlServerInfo>
        {
            public SqlServerInfoMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.UserData1).Optional();
                Map(m => m.UserData2).Optional();
                Map(m => m.Scope).Optional().Default("");
            }
        }
    }
}
