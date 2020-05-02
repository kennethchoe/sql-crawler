using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using LibGit2Sharp;

namespace SqlCrawler.Backend
{
    public class SqlSourceReader
    {
        private const string Path = "sql-source";

        private readonly IAppConfig _appConfig;

        public SqlSourceReader(IAppConfig appConfig)
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

        public Dictionary<string, string> Read()
        {
            if (Directory.Exists(Path))
            {
                SetAttributesNormal(new DirectoryInfo(Path));
                Directory.Delete(Path, true);
            }
            Directory.CreateDirectory(Path);
            Repository.Clone(_appConfig.SqlSourceGitRepoPath, Path);

            var result = new Dictionary<string, string>();

            foreach (var file in Directory.GetFiles(Path))
            {
                if (file.ToLower().EndsWith(".sql"))
                {
                    result.Add(file.Substring(Path.Length + 1, file.Length - Path.Length - 5), File.ReadAllText(file));
                }
            }

            return result;
        }
    }
}
