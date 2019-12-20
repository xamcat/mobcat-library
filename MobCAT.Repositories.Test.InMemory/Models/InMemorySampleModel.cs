using Microsoft.MobCAT.Repositories.InMemory;

namespace Microsoft.MobCAT.Repositories.Test.InMemory.Models
{
    public class InMemorySampleModel : BaseInMemoryModel
    {
        public string SampleString { get; set; }
        public int SampleInt { get; set; }
        public long TimestampTicks { get; set; }
    }
}