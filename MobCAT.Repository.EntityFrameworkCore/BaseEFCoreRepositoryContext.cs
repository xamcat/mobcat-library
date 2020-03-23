using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore
{
    public class BaseEFCoreRepositoryContext : BaseRepositoryContext<SqliteConnection>
    {
        Task<SqliteConnection> _openConnectionTask;

        public BaseEFCoreRepositoryContext(string folderPath, string datastoreName)
            : base(folderPath, datastoreName)
        {
            SQLitePCL.Batteries_V2.Init();  
        }

        protected override Task OnCloseConnectionAsync()
        {
            _openConnectionTask = null;
            Connection?.Close();

            return Task.FromResult(true);
        }

        protected override Task<SqliteConnection> OnOpenConnectionAsync(string datastoreFilepath)
        {
            if (_openConnectionTask == null || _openConnectionTask.IsCompleted)
                _openConnectionTask = OpenConnectionTask(datastoreFilepath);

            return _openConnectionTask;
        }

        async Task<SqliteConnection> OpenConnectionTask(string datastoreFilepath)
        {
            // TODO: Review progess on this Xamarin performance issue: https://github.com/aspnet/EntityFrameworkCore/issues/12087
            // See: https://github.com/aspnet/EntityFrameworkCore/issues/12087#issuecomment-496304143
            
            var connection = new SqliteConnection($"Filename={datastoreFilepath}");

            // WAL mode only available in version 3.0+ (https://github.com/aspnet/EntityFrameworkCore/issues/14059)

            // Post-upgrade to 3.1.2, WAL should be enabled by default on databases created using Entity Framework Core.
            // See: https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/async

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            return connection;
        }
    }
}