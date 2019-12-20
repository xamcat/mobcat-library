using Microsoft.MobCAT.Repository.InMemory;

namespace Microsoft.MobCAT.Repository.Test.InMemory.Models
{
    public class InMemorySampleModel : BaseInMemoryModel
    {
        public string SampleString { get; set; }
        public int SampleInt { get; set; }
        public long TimestampTicks { get; set; }
    }
}