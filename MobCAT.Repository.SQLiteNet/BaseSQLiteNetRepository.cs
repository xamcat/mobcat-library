using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using Microsoft.MobCAT.Extensions;

namespace Microsoft.MobCAT.Repository.SQLiteNet
{
    public class BaseSQLiteNetRepository<T, T2> : BaseRepository<T, T2, SQLiteException> where T2 : BaseSQLiteNetModel, new()
    {
        public BaseSQLiteNetRepository(SQLiteAsyncConnection connection)
        {
            Connection = Guard.Null(connection);
            InitializeAsync(); 
        }

        public SQLiteAsyncConnection Connection { get; private set; }

        protected override Task OnInitializeAsync()
            => Connection.CreateTableAsync<T2>();

        protected override Task OnDropTableAsync()
            => Connection.DropTableAsync<T2>();

        protected override async Task<IEnumerable<T2>> OnExecuteTableQueryAsync(Expression<Func<T2, bool>> expression = null)
            => (await Connection.Table<T2>().Where(expression).ToListAsync()) as IEnumerable<T2>;

        protected override Task<T2> OnExecuteTableQueryScalarAsync(Expression<Func<T2, bool>> expression = null)
            => Connection.Table<T2>().FirstOrDefaultAsync(expression);

        protected override async Task<IEnumerable<T2>> OnGetAsync()
            => (await Connection.Table<T2>().ToListAsync()) as IEnumerable<T2>;

        protected override Task<T2> OnGetItemAsync(string id)
            => Connection.Table<T2>().FirstOrDefaultAsync(i => i.Id == id);

        protected override Task OnInsertAsync(IEnumerable<T2> items)
            => Connection.InsertAllAsync(items);

        protected override Task OnInsertItemAsync(T2 item)
            => Connection.InsertAsync(item);

        protected override Task OnUpdateAsync(IEnumerable<T2> items)
            => Connection.UpdateAllAsync(items);

        protected override Task OnUpdateItemAsync(T2 item)
            => Connection.UpdateAsync(item);

        protected override Task OnUpsertAsync(IEnumerable<T2> items)
            => Connection.RunInTransactionAsync((SQLiteConnection conn) => items.ForEach(i => conn.InsertOrReplace(i)));

        protected override Task OnUpsertItemAsync(T2 item)
            => Connection.InsertOrReplaceAsync(item);

        protected override Task OnRemoveAsync(IEnumerable<T2> items)
            => Connection.RunInTransactionAsync((SQLiteConnection conn) => items.ForEach(i => conn.Delete(i)));

        protected override Task OnRemoveAllAsync()
            => Connection.DeleteAllAsync<T2>();

        protected override Task OnRemoveItemAsync(T2 item)
            => Connection.DeleteAsync(item);
    }
}