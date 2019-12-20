using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.MobCAT.Repository.Abstractions
{
    public interface IBaseRepository<T>
    {
        Task InitializeAsync();
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetItemAsync(string id);
        Task InsertAsync(IEnumerable<T> items);
        Task InsertItemAsync(T item);
        Task UpdateAsync(IEnumerable<T> items);
        Task UpdateItemAsync(T item);
        Task UpsertAsync(IEnumerable<T> items);
        Task UpsertItemAsync(T item);
        Task RemoveAsync(IEnumerable<T> items);
        Task RemoveItemAsync(T item);
        Task RemoveAllAsync();
        Task DropTableAsync();
    }
}