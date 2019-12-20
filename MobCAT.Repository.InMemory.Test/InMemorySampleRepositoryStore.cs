using Microsoft.MobCAT.Repository.InMemory;
using Microsoft.MobCAT.Repository.Test.Abstractions;
using Microsoft.MobCAT.Repository.Test.InMemory.Repositories;

namespace Microsoft.MobCAT.Repository.InMemory.Test
{
    public class InMemorySampleRepositoryContext : BaseInMemoryRepositoryContext, ISampleRepositoryContext
    {
        ISampleRepository _sampleRepository;

        public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new InMemorySampleRepository());

        protected override void OnResetRepositories()
            => _sampleRepository = null;
    }
}