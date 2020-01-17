using Microsoft.MobCAT.Repository.Abstractions;

namespace Microsoft.MobCAT.Repository.Test.Abstractions
{
    public interface ISampleRepositoryContext : IRepositoryContext
    {
        ISampleRepository SampleRepository { get; }
    }
}