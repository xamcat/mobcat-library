using System.Threading.Tasks;

namespace Microsoft.MobCAT.Repository.Abstractions
{
    public interface IRepositoryContext
    {
        Task SetupAsync();
        Task ResetAsync();
        Task DeleteAsync();
    }
}