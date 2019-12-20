using Microsoft.MobCAT.Repository.InMemory.Test.Repositories;
using Microsoft.MobCAT.Repository.Test.Abstractions;

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