using Newtonsoft.Json;

namespace Microsoft.MobCAT.Converters
{
    /// <summary>
    /// Newtonsoft.Json string serializer.
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer<string>
    {
        public string MediaType => "application/json";

        /// <inheritdoc />
        public T Deserialize<T>(string value)
            => JsonConvert.DeserializeObject<T>(value);

        /// <inheritdoc />
        public string Serialize<T>(T value)
            => JsonConvert.SerializeObject(value);
    }
}