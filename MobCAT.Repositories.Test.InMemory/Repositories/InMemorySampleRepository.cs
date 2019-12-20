using System;
using Microsoft.MobCAT.Repositories.InMemory;
using Microsoft.MobCAT.Repositories.Test.Abstractions;
using Microsoft.MobCAT.Repositories.Test.InMemory.Models;
using Microsoft.MobCAT.Repositories.Test.Models;

namespace Microsoft.MobCAT.Repositories.Test.InMemory.Repositories
{
    public class InMemorySampleRepository : BaseInMemoryRepository<SampleModel, InMemorySampleModel>, ISampleRepository
    {
        protected override SampleModel ToModelType(InMemorySampleModel repositoryType)
            => repositoryType == null ? null : new SampleModel
            {
                Id = repositoryType.Id,
                SampleStringProperty = repositoryType.SampleString,
                SampleIntProperty = repositoryType.SampleInt,
                Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero)
            };

        protected override InMemorySampleModel ToRepositoryType(SampleModel modelType)
            => modelType == null ? null : new InMemorySampleModel
            {
                Id = modelType.Id,
                SampleString = modelType.SampleStringProperty,
                SampleInt = modelType.SampleIntProperty,
                TimestampTicks = modelType.Timestamp.UtcTicks
            };
    }
}