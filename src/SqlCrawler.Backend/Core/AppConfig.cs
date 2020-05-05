namespace SqlCrawler.Backend.Core
{
    public interface IAppConfig
    {
        string[] SqlCredentialsFilePath { get; }
        string SqlSourceGitRepoUrl { get; }
        string[] SqlSourceGitClonePath { get; }
        int? CommandTimeoutInSeconds { get; }
        string[] SqliteDataPath { get; }
        string SqlSourceGitUsername { get; }
        string SqlSourceGitPassword { get; }
    }

    public class AppConfig: IAppConfig
    {
        public string[] SqlCredentialsFilePath { get; set; }
        public string SqlSourceGitRepoUrl { get; set; }
        public string[] SqlSourceGitClonePath { get; set; }
        public int? CommandTimeoutInSeconds { get; set; }
        public string[] SqliteDataPath { get; set; }
        public string SqlSourceGitUsername { get; set; }
        public string SqlSourceGitPassword { get; set; }
    }
}
