using System.Threading.Tasks;
using SQLite;

namespace Microsoft.MobCAT.Repository.SQLiteNet
{
    public class BaseSQLiteNetRepositoryContext : BaseRepositoryContext<SQLiteAsyncConnection>
    {
        Task<SQLiteAsyncConnection> _openConnectionTask;

        public BaseSQLiteNetRepositoryContext(string folderPath, string datastoreName)
            : base(folderPath, datastoreName) { }

        protected override Task OnCloseConnectionAsync()
        {
            _openConnectionTask = null;
            return Connection?.CloseAsync();
        }

        protected override Task<SQLiteAsyncConnection> OnOpenConnectionAsync(string datastoreFilepath)
        {
            if (_openConnectionTask == null || _openConnectionTask.IsCompleted)
                _openConnectionTask = OpenConnectionTask(datastoreFilepath);

            return _openConnectionTask;
        }

        async Task<SQLiteAsyncConnection> OpenConnectionTask(string datastoreFilepath)
        {
            var connection = new SQLiteAsyncConnection(datastoreFilepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, true);
            await connection.EnableWriteAheadLoggingAsync().ConfigureAwait(false); // TODO: Review why this causes issues on Android!! (https://github.com/praeclarum/sqlite-net/issues/700 | https://github.com/praeclarum/sqlite-net/issues/757)

            return connection;
        }
    }
}