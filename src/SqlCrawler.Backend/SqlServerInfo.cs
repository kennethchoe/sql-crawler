namespace SqlCrawler.Backend
{
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