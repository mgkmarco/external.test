using Confluent.Kafka;
using Confluent.Kafka.Admin;
using External.Test.Contracts.Factories;
using System;

namespace External.Test.Factories
{
    public class KafkaTopicFactory : IKafkaTopicFactory
    {
        private readonly IAdminClient _adminClient;

        public KafkaTopicFactory(ProducerConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            
            _adminClient = new AdminClientBuilder(config).Build();
        }

        public bool CreateTopic(TopicSpecification topicSpecification)
        {
            if (topicSpecification == null)
            {
                throw new ArgumentNullException(nameof(topicSpecification));
            }

            var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(30));

            if (!metadata.Topics.Exists(x => x.Topic == topicSpecification.Name))
            {
                try
                {
                    _adminClient.CreateTopicsAsync(new[]
                    {
                        topicSpecification
                    }).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    //just in case we have a race condition
                    if (!e.Message.Contains("already exists"))
                    {
                        throw;   
                    }
                }
            }

            return true;
        }
    }
}