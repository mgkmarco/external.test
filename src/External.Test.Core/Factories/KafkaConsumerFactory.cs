using Confluent.Kafka;
using External.Test.Contracts.Constants;
using External.Test.Contracts.Factories;
using External.Test.Contracts.Options;
using External.Test.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace External.Test.Factories
{
    public class KafkaConsumerFactory : IKafkaConsumerFactory
    {
        public IConsumer<TKey, TValue> CreateKafkaConsumer<TKey, TValue>(IConfigurationSection configurationSection, 
            ILogger<IConsumer<TKey, TValue>> logger)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }
            
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var connectionString = configurationSection.GetValue<string>(Section.ConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var consumerConfigurationSection = configurationSection.GetSection(Section.Consumers);

            if (consumerConfigurationSection == null)
            {
                throw new ArgumentNullException(nameof(consumerConfigurationSection));
            }

            var consumerOptions = new List<ConsumerOptions>();
            consumerConfigurationSection.Bind(consumerOptions);

            var consumer = consumerOptions.FirstOrDefault(x =>
                string.Equals(x.MessageType, typeof(TValue).Name, StringComparison.InvariantCultureIgnoreCase));

            if (consumer == null)
            {
                throw new ArgumentNullException($"No consumer configuration for the message type found");
            }

            var consumerConfig = new ConsumerConfig(consumer.Configurations) {BootstrapServers = connectionString};
            
            var kafkaConsumer = new ConsumerBuilder<TKey, TValue>(consumerConfig)
                .SetKeyDeserializer(new JsonDeserializer<TKey>())
                .SetValueDeserializer(new JsonDeserializer<TValue>())
                .SetErrorHandler((x, e) => logger.LogError($"ErrorCode: {e.Code}, Reason: {e.Reason}"))
                .SetLogHandler((x, e) => logger.LogInformation($"LogLevel: {e.Level}, LogName: {e.Name}, LogMessage: {e.Message}"))
                .Build();

            return kafkaConsumer;
        }
    }
}