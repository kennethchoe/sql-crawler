using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend
{
    public class SqlQueryReader
    {
        private const string Path = "sql-source";

        private readonly IAppConfig _appConfig;

        public SqlQueryReader(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        // https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it
        private void SetAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                SetAttributesNormal(subDir);
                subDir.Attributes = FileAttributes.Normal;
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }

        private CloneOptions BuildCloneOption()
        {
            if (string.IsNullOrEmpty(_appConfig.SqlSourceGitUsername)) return new CloneOptions();
            
            var credentials = new UsernamePasswordCredentials
            {
                Username = _appConfig.SqlSourceGitUsername,
                Password = _appConfig.SqlSourceGitPassword
            };

            return new CloneOptions
            {
                CredentialsProvider = (s, fromUrl, types) => credentials
            };
        }

        public void Reload()
        {
            if (Directory.Exists(Path))
            {
                SetAttributesNormal(new DirectoryInfo(Path));
                Directory.Delete(Path, true);
            }
            Directory.CreateDirectory(Path);


            Repository.Clone(_appConfig.SqlSourceGitRepoPath, Path, BuildCloneOption());
        }

        public IEnumerable<SqlQueryInfo> Read()
        {
            if (!Directory.Exists(Path)) Reload();

            var result = new List<SqlQueryInfo>();

            foreach (var file in Directory.GetFiles(Path))
            {
                if (file.ToLower().EndsWith(".sql"))
                {
                    result.Add(new SqlQueryInfo {
                        Name = file.Substring(Path.Length + 1, file.Length - Path.Length - 5), 
                        Query = File.ReadAllText(file)
                            });
                }
            }

            return result;
        }
    }
}
