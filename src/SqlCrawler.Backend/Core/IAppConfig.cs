namespace SqlCrawler.Backend.Core
{
    public interface IAppConfig
    {
        string[] SqlCredentialsFilePath { get; }
        string SqlSourceGitRepoUrl { get; }
        string[] SqlSourceGitClonePath { get; }
        int? CommandTimeoutInSeconds { get; }
        int? ConnectionTimeoutInSeconds { get; }
        string[] SqliteDataPath { get; }
        string SqlSourceGitUsername { get; }
        string SqlSourceGitPassword { get; }
    }
}