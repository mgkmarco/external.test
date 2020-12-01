using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace External.Test.Contracts.Factories
{
    public interface IKafkaConsumerFactory
    {
        IConsumer<TKey, TValue> CreateKafkaConsumer<TKey, TValue>(IConfigurationSection configurationSection,
            ILogger<IConsumer<TKey, TValue>> logger);
    }
}