namespace SqlCrawler.Backend.Core
{
    public class SqlServerInfo: SqlServerInfoPublic
    {
        public string DataSource { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
    }
}