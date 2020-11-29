using Confluent.Kafka.Admin;

namespace External.Test.Contracts.Factories
{
    public interface IKafkaTopicFactory
    {
        bool CreateTopic(TopicSpecification topicSpecification);
    }
}