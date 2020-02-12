using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore
{
    public class BaseEFCoreRepository<T, T2> : BaseRepository<T, T2, SqliteException> where T2 : BaseEFCoreModel, new()
    {
        public BaseEFCoreRepository(SqliteConnection connection)
        {
            Connection = Guard.Null(connection);
            InitializeAsync();
        }

        public SqliteConnection Connection { get; private set; }

        public RepositoryDbContext<T2> Context => GetContext();

        protected override async Task OnInitializeAsync()
        {
            try
            {
                using (var context = GetContext())
                {
                    var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
                    await databaseCreator.CreateTablesAsync().ConfigureAwait(false);
                }
            }
            catch (SqliteException) { }
        }

        protected override Task OnDropTableAsync()
            => OnRemoveAllAsync(); 

        protected override async Task<IEnumerable<T2>> OnExecuteTableQueryAsync(Expression<Func<T2, bool>> expression = null)
        {
            IEnumerable<T2> results = null;

            using (var context = GetContext())
                results = await context.Items.Where(expression).ToListAsync().ConfigureAwait(false);

            return results;
        }

        protected override async Task<IEnumerable<IGrouping<TGroupKey, T2>>> OnExecuteTableQueryAsync<TGroupKey>(Expression<Func<T2, bool>> expression = null, Expression<Func<T2, TGroupKey>> groupingExpression = null)
        {
            IEnumerable<IGrouping<TGroupKey,T2>> results = default;

            using (var context = GetContext())
                results = await context.Items.Where(expression).GroupBy(groupingExpression).ToListAsync().ConfigureAwait(false);

            return results;
        }

        protected override async Task<T2> OnExecuteTableQueryScalarAsync(Expression<Func<T2, bool>> expression = null)
        {
            T2 result = null;

            using (var context = GetContext())
                result = await context.Items.FirstOrDefaultAsync(expression).ConfigureAwait(false);

            return result;
        }

        protected override async Task<IEnumerable<T2>> OnGetAsync()
        {
            IEnumerable<T2> results = null;

            using (var context = GetContext())
                results = await context.Items.ToListAsync().ConfigureAwait(false);

            return results;
        }

        protected override async Task<T2> OnGetItemAsync(string id)
        {
            T2 result = null;

            using (var context = GetContext())
                result = await context.Items.FirstOrDefaultAsync<T2>(i => i.Id == id).ConfigureAwait(false);

            return result;
        }

        protected override async Task OnInsertAsync(IEnumerable<T2> items)
        {
            using (var context = GetContext())
            {
                await context.Items.AddRangeAsync(items).ConfigureAwait(false);
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnInsertItemAsync(T2 item)
        {
            using (var context = GetContext())
            {
                await context.Items.AddAsync(item).ConfigureAwait(false);
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnUpdateAsync(IEnumerable<T2> items)
        {
            using (var context = GetContext())
            {
                await Task.Run(() => context.Items.UpdateRange(items)).ConfigureAwait(false);
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnUpdateItemAsync(T2 item)
        {
            using (var context = GetContext())
            {
                await Task.Run(() => context.Items.Update(item)).ConfigureAwait(false);
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        // TODO: Review whether to roll our own as this is not supported in EFCore
        // See: https://github.com/aspnet/EntityFrameworkCore/issues/4526#issuecomment-347270847
        // This is in the backlog: https://github.com/aspnet/EntityFrameworkCore/issues/4526#issuecomment-470988951

        protected override async Task OnUpsertAsync(IEnumerable<T2> items)
        {
            // TODO: Compare performance of approaches (see single upsert approach below)!!
            using (var context = GetContext())
            {
                var idValues = items.Select(i => i.Id).ToList();
                var existingItems = await context.Items.Where(i => idValues.Contains(i.Id)).ToListAsync().ConfigureAwait(false);
                var existingIdValues = existingItems.Select(i => i.Id).ToList();
                var newItems = items.Where(i => !existingIdValues.Contains(i.Id));

                if (newItems.Any())
                    await context.Items.AddRangeAsync(newItems).ConfigureAwait(false);

                if (existingItems.Any())
                    await Task.Run(() => context.Items.UpdateRange(items)).ConfigureAwait(false);

                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnUpsertItemAsync(T2 item)
        {
            using (var context = GetContext())
            {
                var existingItem = await context.Items.FirstOrDefaultAsync(i => i.Id == item.Id).ConfigureAwait(false);

                if (existingItem == null || EqualityComparer<T2>.Default.Equals(default(T2), existingItem))
                    await context.Items.AddAsync(item).ConfigureAwait(false);
                else
                    await Task.Run(() => context.Items.Update(item)).ConfigureAwait(false);

                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnRemoveAsync(IEnumerable<T2> items)
        {
            using (var context = GetContext())
            {
                await Task.Run(() => context.Items.RemoveRange(items));
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnRemoveAllAsync()
        {
            var allItems = await OnGetAsync().ConfigureAwait(false);

            if (!allItems.Any())
                return;

            using (var context = GetContext())
            {
                await Task.Run(() => context.Items.RemoveRange(allItems));
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        protected override async Task OnRemoveItemAsync(T2 item)
        {
            using (var context = GetContext())
            {
                await Task.Run(() => context.Remove(item)).ConfigureAwait(false);
                await context.SaveChangesAsync(true).ConfigureAwait(false);
            }
        }

        Func<RepositoryDbContext<T2>> GetContext => () => new RepositoryDbContext<T2>(Connection);
    }
}