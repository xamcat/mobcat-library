using System;
using Microsoft.Data.Sqlite;
using Microsoft.MobCAT.Repository.EntityFrameworkCore.Test.Models;
using Microsoft.MobCAT.Repository.Test.Abstractions;
using Microsoft.MobCAT.Repository.Test.Models;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore.Test.Repositories
{
    public class EFCoreSampleRepository : BaseEFCoreRepository<SampleModel, EFCoreSampleModel>, ISampleRepository
    {
        public EFCoreSampleRepository(SqliteConnection connection)
            : base(connection) { }

        protected override SampleModel ToModelType(EFCoreSampleModel repositoryType)
            => repositoryType == null ? null : new SampleModel
            {
                Id = repositoryType.Id,
                SampleStringProperty = repositoryType.SampleString,
                SampleIntProperty = repositoryType.SampleInt,
                Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero)
            };

        protected override EFCoreSampleModel ToRepositoryType(SampleModel modelType)
            => modelType == null ? null : new EFCoreSampleModel
            {
                Id = modelType.Id,
                SampleString = modelType.SampleStringProperty,
                SampleInt = modelType.SampleIntProperty,
                TimestampTicks = modelType.Timestamp.UtcTicks
            };
    }
}