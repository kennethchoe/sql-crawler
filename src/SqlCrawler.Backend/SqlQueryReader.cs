using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend
{
    public class SqlQueryReader
    {
        private readonly IAppConfig _appConfig;
        private readonly string _clonePath;

        public SqlQueryReader(IAppConfig appConfig)
        {
            _appConfig = appConfig;
            _clonePath = Path.Combine(_appConfig.SqlSourceGitClonePath ?? new[] { "sql-source" });
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

            foreach (var file in Directory.GetFiles(_clonePath, "*.sql", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(_clonePath.Length + 1);
                var info = Parse(relativePath);
                result.Add(new SqlQueryInfo {
                    Scope = info.Scope,
                    Name = info.Name, 
                    Query = File.ReadAllText(file)
                        });
            }

            var duplicates = result.GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();
            if (duplicates.Any())
                throw new Exception("SqlQuery filename must be unique. Duplicated name(s): " +
                                                  duplicates.Select(x => x.Key).Aggregate((x, y) => x + ", " + y));

            return result;
        }

        public QueryFileInfo Parse(string relativePath)
        {
            return new QueryFileInfo
            {
                Scope = Path.GetDirectoryName(relativePath).Replace("\\", "/"),
                Name = Path.GetFileNameWithoutExtension(relativePath)
            };
        }
    }

    public class QueryFileInfo
    {
        public string Scope { get; set; }
        public string Name { get; set; }
    }
}
