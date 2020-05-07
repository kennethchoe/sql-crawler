namespace SqlCrawler.Backend.Core
{
    public class SqlServerInfo: SqlServerInfoPublic
    {
        public string DataSource { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }

        public string ToConnectionString(int? connectionTimeout)
        {
            var result = "Data Source=" + DataSource + ";";

            if (UseWindowsAuthentication)
                result += "Integrated Security=SSPI;";
            else
                result += $"User Id={SqlUsername};Password={SqlPassword};";

            if (connectionTimeout != null)
                result += "Connect Timeout=" + connectionTimeout + ";";

            return result;
        }
    }
}