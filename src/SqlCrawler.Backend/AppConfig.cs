namespace SqlCrawler.Backend
{
    public interface IAppConfig
    {
        string SqlCredentialsFilePath { get; }
        string SqlSourceGitRepoPath { get; }
        int? CommandTimeoutInSeconds { get; set; }
    }

    public class AppConfig: IAppConfig
    {
        public string SqlCredentialsFilePath { get; set; }
        public string SqlSourceGitRepoPath { get; set; }
        public int? CommandTimeoutInSeconds { get; set; }
    }
}
