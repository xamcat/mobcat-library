using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.MobCAT.Extensions;

namespace Microsoft.MobCAT.Repository.InMemory
{
    /// <summary>
    /// Basic in-memory repository to aid in testing and prototyping.
    /// </summary>
    /// <typeparam name="T">The domain object.</typeparam>
    /// <typeparam name="T2">The entity object.</typeparam>
    public class BaseInMemoryRepository<T, T2> : BaseRepository<T, T2, Exception> where T2 : BaseInMemoryModel
    {
        ConcurrentDictionary<string, T2> _items;

        public BaseInMemoryRepository()
            => InitializeAsync();

        public ConcurrentDictionary<string, T2> Items
        {
            get => _items ?? (_items = new ConcurrentDictionary<string, T2>());
            private set => _items = value;
        }

        protected override Task OnInitializeAsync()
            => Task.FromResult(true);

        protected override Task OnDropTableAsync()
            => Task.Run(() => Items = null);

        protected override Task<IEnumerable<T2>> OnExecuteTableQueryAsync(Expression<Func<T2, bool>> expression = null)
            => Task.Run(() => Items.Select(i => i.Value).Where(expression.Compile()).ToList() as IEnumerable<T2>);

        protected override Task<IEnumerable<IGrouping<TGroupKey, T2>>> OnExecuteTableQueryAsync<TGroupKey>(Expression<Func<T2, bool>> expression = null, Expression<Func<T2, TGroupKey>> groupingExpression = null)
            => Task.Run(() => Items.Select(i => i.Value).Where(expression.Compile()).GroupBy<T2, TGroupKey>(groupingExpression.Compile()));

        protected override Task<T2> OnExecuteTableQueryScalarAsync(Expression<Func<T2, bool>> expression = null)
            => Task.Run(() => Items.Select(i => i.Value).FirstOrDefault(expression.Compile()));

        protected override Task<IEnumerable<T2>> OnGetAsync()
            => Task.Run(() => Items.Select(i => i.Value).OrderBy(i => i.Id).ToList() as IEnumerable<T2>);

        protected override Task<T2> OnGetItemAsync(string id)
            => Task.Run(() => { return !Items.TryGetValue(id, out var item) ? null : item; });

        protected override Task OnInsertAsync(IEnumerable<T2> items)
            => Task.Run(() => items.ForEach(AddItem));

        protected override Task OnInsertItemAsync(T2 item)
            => Task.Run(() => AddItem(item));

        protected override Task OnUpdateAsync(IEnumerable<T2> items)
           => Task.Run(() => items.ForEach(UpdateItem));

        protected override Task OnUpdateItemAsync(T2 item)
           => Task.Run(() => UpdateItem(item));

        protected override Task OnUpsertAsync(IEnumerable<T2> items)
            => Task.Run(() => items.ForEach(UpsertItem));

        protected override Task OnUpsertItemAsync(T2 item)
            => Task.Run(() => UpsertItem(item));

        protected override Task OnRemoveAsync(IEnumerable<T2> items)
            => Task.Run(() => items.ForEach(RemoveItem));

        protected override Task OnRemoveAllAsync()
            => Task.Run(() => Items.Clear());

        protected override Task OnRemoveItemAsync(T2 item)
            => Task.Run(() => RemoveItem(item));

        void AddItem(T2 item)
        {
            if (!Items.TryAdd(item.Id, item))
                throw new Exception("The specified item could not be added");
        }

        void UpdateItem(T2 item)
        {
            if (!Items.TryGetValue(item.Id, out var currentItem))
                throw new Exception("The specified item does not exist");

            if (!Items.TryUpdate(item.Id, item, currentItem))
                throw new Exception("The specified item could not be updated");
        }

        void UpsertItem(T2 item)
        {
            if (Items.AddOrUpdate(item.Id, item, (id, oldValue) => item) == null)
                throw new Exception("The specified item could not be upserted");
        }

        void RemoveItem(T2 item)
        {
            if (!Items.TryRemove(item.Id, out var removedItem))
                throw new Exception($"The specified item, with id {removedItem.Id}, could not be removed");
        }
    }
}