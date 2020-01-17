using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.MobCAT.Repository.Abstractions;
using Polly;

namespace Microsoft.MobCAT.Repository
{
    public class BaseRepository<T, T2, T3> : IBaseRepository<T> where T3 : Exception
    {
        private Task _initializeTask;
        protected Task RepositoryInitializationTask => _initializeTask ?? (_initializeTask = Task.Run(OnInitializeAsync));

        protected virtual T ToModelType(T2 repositoryType) => default(T);
        protected virtual T2 ToRepositoryType(T modelType) => default(T2);
        protected virtual Task<IEnumerable<T2>> OnExecuteTableQueryAsync(Expression<Func<T2, bool>> expression = null) => throw new NotImplementedException();
        protected virtual Task<T2> OnExecuteTableQueryScalarAsync(Expression<Func<T2, bool>> expression = null) => throw new NotImplementedException();
        protected virtual Task OnInitializeAsync() => throw new NotImplementedException();
        protected virtual Task OnDropTableAsync() => throw new NotImplementedException();
        protected virtual Task<IEnumerable<T2>> OnGetAsync() => throw new NotImplementedException();
        protected virtual Task<T2> OnGetItemAsync(string id) => throw new NotImplementedException();
        protected virtual Task OnInsertAsync(IEnumerable<T2> items) => throw new NotImplementedException();
        protected virtual Task OnInsertItemAsync(T2 item) => throw new NotImplementedException();
        protected virtual Task OnUpsertAsync(IEnumerable<T2> items) => throw new NotImplementedException();
        protected virtual Task OnUpsertItemAsync(T2 item) => throw new NotImplementedException();
        protected virtual Task OnRemoveAsync(IEnumerable<T2> items) => throw new NotImplementedException();
        protected virtual Task OnRemoveItemAsync(T2 item) => throw new NotImplementedException();
        protected virtual Task OnRemoveAllAsync() => throw new NotImplementedException();
        protected virtual Task OnUpdateAsync(IEnumerable<T2> items) => throw new NotImplementedException();
        protected virtual Task OnUpdateItemAsync(T2 item) => throw new NotImplementedException();

        protected virtual AsyncPolicy OnGetRetryPolicy()
        {
            return Policy
                .Handle<T3>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        protected IEnumerable<T> ToModelType(IEnumerable<T2> repositoryTypes)
            => repositoryTypes.Select(ToModelType);

        protected IEnumerable<T2> ToRepositoryType(IEnumerable<T> modelTypes)
            => modelTypes.Select(ToRepositoryType);

        protected Task<IEnumerable<T2>> ExecuteTableQueryAsync(Expression<Func<T2, bool>> expression = null)
            => OnExecuteTableQueryAsync(expression);

        protected Task<T2> ExecuteTableQueryScalarAsync(Expression<Func<T2, bool>> expression = null)
            => OnExecuteTableQueryScalarAsync(expression);

        public Task InitializeAsync() => RepositoryInitializationTask;

        public Task DropTableAsync()
        {
            _initializeTask = null;
            return OnDropTableAsync();
        }

        public AsyncPolicy GetRetryPolicy()
            => OnGetRetryPolicy();

        public async Task<IEnumerable<T>> GetAsync()
        {
            await InitializeAsync().ConfigureAwait(false);
            var items = await GetRetryPolicy().ExecuteAsync(() => OnGetAsync()).ConfigureAwait(false);

            return items?.Select(ToModelType).ToList();
        }

        public async Task<T> GetItemAsync(string id)
        {
            Guard.NullOrWhitespace(id);
            await InitializeAsync().ConfigureAwait(false);
            var item = await GetRetryPolicy().ExecuteAsync(() => OnGetItemAsync(id)).ConfigureAwait(false);

            return ToModelType(item);
        }

        public async Task InsertAsync(IEnumerable<T> items)
        {
            Guard.Null(items);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItems = await Task.Run(() => { return items.Select(ToRepositoryType).ToList(); }).ConfigureAwait(false);
            var test = GetRetryPolicy();
            await GetRetryPolicy().ExecuteAsync(() => OnInsertAsync(convertedItems)).ConfigureAwait(false);
        }

        public async Task InsertItemAsync(T item)
        {
            Guard.Null(item);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItem = await Task.Run(() => { return ToRepositoryType(item); }).ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnInsertItemAsync(convertedItem)).ConfigureAwait(false);
        }

        public async Task UpsertAsync(IEnumerable<T> items)
        {
            Guard.Null(items);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItems = await Task.Run(() => { return ToRepositoryType(items); }).ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnUpsertAsync(convertedItems)).ConfigureAwait(false);
        }

        public async Task UpsertItemAsync(T item)
        {
            Guard.Null(item);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItem = await Task.Run(() => { return ToRepositoryType(item); }).ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnUpsertItemAsync(convertedItem)).ConfigureAwait(false);
        }

        public async Task RemoveAsync(IEnumerable<T> items)
        {
            Guard.Null(items);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItems = await Task.Run(() => { return items.Select(ToRepositoryType).ToList(); }).ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnRemoveAsync(convertedItems)).ConfigureAwait(false);
        }

        public async Task RemoveAllAsync()
        {
            await InitializeAsync().ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnRemoveAllAsync()).ConfigureAwait(false);
        }

        public async Task RemoveItemAsync(T item)
        {
            Guard.Null(item);
            await InitializeAsync().ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnRemoveItemAsync(ToRepositoryType(item))).ConfigureAwait(false);
        }

        public async Task UpdateAsync(IEnumerable<T> items)
        {
            Guard.Null(items);
            await InitializeAsync().ConfigureAwait(false);
            var convertedItems = await Task.Run(() => { return items.Select(ToRepositoryType).ToList(); }).ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnUpdateAsync(convertedItems)).ConfigureAwait(false);
        }

        public async Task UpdateItemAsync(T item)
        {
            Guard.Null(item);
            await InitializeAsync().ConfigureAwait(false);
            await GetRetryPolicy().ExecuteAsync(() => OnUpdateItemAsync(ToRepositoryType(item))).ConfigureAwait(false);
        }
    }
}