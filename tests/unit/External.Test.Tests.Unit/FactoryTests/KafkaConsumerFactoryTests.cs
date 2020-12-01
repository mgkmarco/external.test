using Confluent.Kafka;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Constants;
using External.Test.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace External.Test.Tests.Unit.FactoryTests
{
    [Trait("Category", "Unit")]
    public class KafkaConsumerFactoryTests
    {
        private readonly Mock<ILogger<IConsumer<int, UpdateMarketCommand>>> _loggerMock;
        private readonly IConfiguration _configuration;

        public KafkaConsumerFactoryTests()
        {
            _loggerMock = new Mock<ILogger<IConsumer<int, UpdateMarketCommand>>>();
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.unit.json")
                .Build();
        }
        
        [Fact]
        public void CreateKafkaConsumer_Failed_NullConfigurationSection_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = new KafkaConsumerFactory();
            
            // Act
            Assert.Throws<ArgumentNullException>(() => sut.CreateKafkaConsumer(null, _loggerMock.Object));
        }
        
        [Fact]
        public void CreateKafkaConsumer_Failed_NullLogger_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = new KafkaConsumerFactory();
            var configurationSection = _configuration.GetSection(Section.Consumers);
            var logger = _loggerMock.Object;
            logger = null;
            
            // Act
            Assert.Throws<ArgumentNullException>(() => sut.CreateKafkaConsumer(configurationSection, logger));
        }
        
        [Fact]
        public void CreateKafkaConsumer_Failed_NullOrEmptyConnectionString_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = new KafkaConsumerFactory();
            var configurationSection = _configuration.GetSection("NoConnection");
            
            // Act
            Assert.Throws<ArgumentNullException>(() => sut.CreateKafkaConsumer(configurationSection, _loggerMock.Object));
        }
        
        [Fact]
        public void CreateKafkaConsumer_Failed_NullConsumerConfigurationSection_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = new KafkaConsumerFactory();
            var configurationSection = _configuration.GetSection("NoConsumers");
            
            // Act
            Assert.Throws<ArgumentNullException>(() => sut.CreateKafkaConsumer(configurationSection, _loggerMock.Object));
        }
    }
}