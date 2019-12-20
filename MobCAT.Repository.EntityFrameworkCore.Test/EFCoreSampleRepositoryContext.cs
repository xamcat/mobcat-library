using Microsoft.MobCAT.Repository.EntityFrameworkCore.Test.Repositories;
using Microsoft.MobCAT.Repository.Test.Abstractions;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore.Test
{
    public class EFCoreSampleRepositoryContext : BaseEFCoreRepositoryContext, ISampleRepositoryContext
    {
        ISampleRepository _sampleRepository;

        public EFCoreSampleRepositoryContext(string folderPath, string datastoreName)
            : base(folderPath, datastoreName) { }

        public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new EFCoreSampleRepository(Connection));

        protected override void OnResetRepositories()
            => _sampleRepository = null;
    }
}