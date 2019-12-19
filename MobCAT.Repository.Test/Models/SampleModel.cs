using System;
using Newtonsoft.Json;

namespace Microsoft.MobCAT.Repository.Test.Models
{
    public class SampleModel : BaseModel
    {
        [JsonProperty("id")]
        public new string Id { get; set; }

        [JsonProperty("sampleStringProperty")]
        public string SampleStringProperty { get; set; }

        [JsonProperty("sampleIntProperty")]
        public int SampleIntProperty { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}