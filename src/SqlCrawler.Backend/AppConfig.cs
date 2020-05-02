namespace SqlCrawler.Backend
{
    public interface IAppConfig
    {
        string SqlCredentialsFilePath { get; }
        string SqlSourceGitRepoPath { get; }
    }

    public class AppConfig: IAppConfig
    {
        public string SqlCredentialsFilePath { get; set; }
        public string SqlSourceGitRepoPath { get; set; }
    }
}
