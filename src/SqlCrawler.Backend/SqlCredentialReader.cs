using System;
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
                throw new Exception("ServerId must be unique. Duplicated Id(s): " + 
                                    duplicates.Select(x => x.Key).Aggregate((x, y) => x + ", " + y));
            
            return records;
        }

        private sealed class SqlServerInfoMap : ClassMap<SqlServerInfo>
        {
            public SqlServerInfoMap()
            {
                Map(m => m.ServerId);
                Map(m => m.ServerName).Optional();
                Map(m => m.Scope).Optional().Default("");
                Map(m => m.Description).Optional();
                Map(m => m.UserData1).Optional();
                Map(m => m.UserData2).Optional();
                Map(m => m.ServerDriver).Optional().Default("mssql");
                Map(m => m.DataSource);
                Map(m => m.UseIntegratedSecurity).Optional().Default("true");
                Map(m => m.SqlUsername).Optional();
                Map(m => m.SqlPassword).Optional();
            }
        }
    }
}
