using Confluent.Kafka;
using System;
using System.Text;
using System.Text.Json;

namespace External.Test.Serializers
{
    public class JsonDeserializer<T> : IDeserializer<T> 
    {
        /// <inheritdoc />
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return default;
            }

            var deserializedDataAsJson = Encoding.UTF8.GetString(data.ToArray());

            return JsonSerializer.Deserialize<T>(deserializedDataAsJson);
        }
    }
}