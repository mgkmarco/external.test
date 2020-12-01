using Confluent.Kafka;
using External.Test.Factories;
using Moq;
using System;
using Xunit;

namespace External.Test.Tests.Unit.FactoryTests
{
    [Trait("Category", "Unit")]
    public class KafkaTopicFactoryTests
    {
        private readonly Mock<ProducerConfig> _configMock;
        
        public KafkaTopicFactoryTests()
        {
            _configMock = new Mock<ProducerConfig>();
        }
        
        [Fact]
        public void Constructor_Failed_NullProducerConfig_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new KafkaTopicFactory(null).CreateTopic(null));
        }
        
        [Fact]
        public void CreateTopic_Failed_NullTopicSpecification_ThrowsArgumentNullException()
        {
            //Arrange 
            var sut = new KafkaTopicFactory(_configMock.Object);
            
            // Act
            Assert.Throws<ArgumentNullException>(() => sut.CreateTopic(null));
        }
    }
}