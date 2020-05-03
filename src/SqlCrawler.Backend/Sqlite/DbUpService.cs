using System;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Helpers;
using Serilog;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Backend.Sqlite
{
    public class DbUpService
    {
        private readonly IAppConfig _config;

        public DbUpService(IAppConfig config)
        {
            _config = config;
        }

        public void Upgrade()
        {
            RunUpgrade(Path.Combine(_config.SqliteDataPath), "Upgrade", useNullJournal: false);
            RunUpgrade(Path.Combine(_config.SqliteDataPath), "AfterEveryUpgrade", useNullJournal: true);
        }

        private static void RunUpgrade(string dataPath, string folder, bool useNullJournal)
        {
            Log.Information("DbUp: " + folder);

            var builder = DeployChanges.To
                .SQLiteDatabase("Data Source=" + dataPath)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                    s => s.Contains("." + folder + "."));

            if (useNullJournal)
                builder.JournalTo(new NullJournal());

            var upgradeService = builder
                .LogToConsole()
                .Build();

            var result = upgradeService.PerformUpgrade();

            if (!result.Successful)
                throw new Exception("DbUp failed.", result.Error);
        }
    }
}