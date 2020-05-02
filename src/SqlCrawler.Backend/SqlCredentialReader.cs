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

    public class SqlServerInfo
    {
        public string ServerId { get; set; }
        public string ServerName { get; set; }
        public string DataSource { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
        public string Description { get; set; }

        public string ToConnectionString()
        {
            var result = "Data Source=" + DataSource + ";";

            if (UseWindowsAuthentication)
                result += "Integrated Security=SSPI;";
            else
                result += $"User Id={SqlUsername};Password={SqlPassword};";

            return result;
        }
    }
}
