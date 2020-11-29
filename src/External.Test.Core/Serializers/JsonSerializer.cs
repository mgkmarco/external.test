using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace External.Test.Serializers
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context) 
            => (object) data == null ? null : Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(data));
    }
}