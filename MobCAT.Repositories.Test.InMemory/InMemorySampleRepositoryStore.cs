using Microsoft.MobCAT.Repositories.InMemory;
using Microsoft.MobCAT.Repositories.Test.Abstractions;
using Microsoft.MobCAT.Repositories.Test.InMemory.Repositories;

namespace Microsoft.MobCAT.Repositories.Test.InMemory
{
    public class InMemorySampleRepositoryStore : BaseInMemoryRepositoryStore, ISampleRepositoryStore
    {
        ISampleRepository _sampleRepository;

        public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new InMemorySampleRepository());

        protected override void OnResetRepositories()
            => _sampleRepository = null;
    }
}