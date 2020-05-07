namespace SqlCrawler.Backend.Core
{
    public class AppConfig: IAppConfig
    {
        public string[] SqlCredentialsFilePath { get; set; }
        public string SqlSourceGitRepoUrl { get; set; }
        public string[] SqlSourceGitClonePath { get; set; }
        public int? CommandTimeoutInSeconds { get; set; }
        public int? ConnectionTimeoutInSeconds { get; set; }
        public string[] SqliteDataPath { get; set; }
        public string SqlSourceGitUsername { get; set; }
        public string SqlSourceGitPassword { get; set; }
    }
}
