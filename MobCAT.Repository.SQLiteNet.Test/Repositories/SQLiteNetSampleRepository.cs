using System;
using Microsoft.MobCAT.Repository.Test.Abstractions;
using Microsoft.MobCAT.Repository.Test.Models;
using Microsoft.MobCAT.Repository.SQLiteNet.Test.Models;
using SQLite;

namespace Microsoft.MobCAT.Repository.SQLiteNet.Test.Repositories
{
    public class SQLiteNetSampleRepository : BaseSQLiteNetRepository<SampleModel, SQLiteNetSampleModel>, ISampleRepository
    {
        public SQLiteNetSampleRepository(SQLiteAsyncConnection connection)
            : base(connection) { }

        protected override SampleModel ToModelType(SQLiteNetSampleModel repositoryType)
            => repositoryType == null ? null : new SampleModel
            {
                Id = repositoryType.Id,
                SampleStringProperty = repositoryType.SampleString,
                SampleIntProperty = repositoryType.SampleInt,
                Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero)
            };

        protected override SQLiteNetSampleModel ToRepositoryType(SampleModel modelType)
            => modelType == null ? null : new SQLiteNetSampleModel
            {
                Id = modelType.Id,
                SampleString = modelType.SampleStringProperty,
                SampleInt = modelType.SampleIntProperty,
                TimestampTicks = modelType.Timestamp.UtcTicks
            };
    }
}