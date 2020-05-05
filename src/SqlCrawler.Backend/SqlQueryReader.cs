using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend
{
    public class SqlQueryReader
    {
        private readonly IAppConfig _appConfig;
        private string _clonePath;

        public SqlQueryReader(IAppConfig appConfig)
        {
            _appConfig = appConfig;
            _clonePath = Path.Combine(_appConfig.SqlSourceGitClonePath);
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

        public void ClearCache()
        {
            if (Directory.Exists(_clonePath))
            {
                SetAttributesNormal(new DirectoryInfo(_clonePath));
                Directory.Delete(_clonePath, true);
            }
        }

        public void Reload()
        {
            ClearCache();
            Directory.CreateDirectory(_clonePath);
            
            Repository.Clone(_appConfig.SqlSourceGitRepoUrl, _clonePath, BuildCloneOption());
        }

        public IEnumerable<SqlQueryInfo> Read()
        {
            if (!Directory.Exists(_clonePath)) Reload();

            var result = new List<SqlQueryInfo>();

            foreach (var file in Directory.GetFiles(_clonePath))
            {
                if (file.ToLower().EndsWith(".sql"))
                {
                    result.Add(new SqlQueryInfo {
                        Name = file.Substring(_clonePath.Length + 1, file.Length - _clonePath.Length - 5), 
                        Query = File.ReadAllText(file)
                            });
                }
            }

            return result;
        }
    }
}
