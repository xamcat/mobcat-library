namespace Microsoft.MobCAT.Repository.Abstractions
{
    public interface IBaseRepositoryContext<T> : IRepositoryContext
    {
        T Connection { get; }
    }
}