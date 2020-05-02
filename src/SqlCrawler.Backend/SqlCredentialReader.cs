using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

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
            using var reader = new StreamReader(_appConfig.SqlCredentialsFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<SqlServerInfo>().ToList();
            return records;
        }
    }
}
