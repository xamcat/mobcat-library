using Microsoft.MobCAT.Repository.SQLiteNet.Test.Repositories;
using Microsoft.MobCAT.Repository.Test.Abstractions;

namespace Microsoft.MobCAT.Repository.SQLiteNet.Test
{
    public class SQLiteNetSampleRepositoryContext : BaseSQLiteNetRepositoryContext, ISampleRepositoryContext
    {
        ISampleRepository _sampleRepository;

        public SQLiteNetSampleRepositoryContext(string folderPath, string datastoreName)
            : base(folderPath, datastoreName) { }

        public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new SQLiteNetSampleRepository(Connection));

        protected override void OnResetRepositories()
            => _sampleRepository = null;
    }
}