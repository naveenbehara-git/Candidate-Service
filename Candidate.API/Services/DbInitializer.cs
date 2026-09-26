using Candidate.Database;
using DbUp;

namespace Candidate.API.Services
{
    public class DbInitializer
    {
        public static void Run(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);
            var assemblies = new[] { typeof(DatabaseScriptPointer).Assembly };

            var upgrader = DeployChanges.To 
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssemblies(assemblies)
                .LogToConsole()
                .Build();
            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                throw result.Error;
            }
        }
    }
}
