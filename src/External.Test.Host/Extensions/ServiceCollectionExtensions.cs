﻿using Confluent.Kafka;
using Confluent.Kafka.Admin;
using External.Test.Contracts.Options;
using External.Test.Data.Contracts.Entities;
using External.Test.Data.Contracts.Repositories;
using External.Test.Data.Repositories;
using External.Test.Factories;
using External.Test.Handlers;
using External.Test.Serializers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace External.Test.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediatrPipeline(this IServiceCollection services)
        {
            services.AddMediatR(typeof(MarketUpdateHandler));
        }
        
        public static void AddKafkaProducer<TKey, TValue>(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var connectionString = configurationSection.GetValue<string>("ConnectionString");
            var producerConfigurationSection = configurationSection.GetSection("Producers");
            
            var producerOptions = new List<ProducerOptions>();
            producerConfigurationSection.Bind(producerOptions);

            var producer = producerOptions.FirstOrDefault(x =>
                string.Equals(x.MessageType, typeof(TValue).Name, StringComparison.InvariantCultureIgnoreCase));

            if (producer == null)
            {
                throw new ArgumentNullException($"No producer configuration for the message type found");
            }
            
            var producerConfig = new ProducerConfig(producer.Configurations){ BootstrapServers = connectionString };
            
            if (producer.EnableTopicCreation)
            {
                var topicSpecification = new TopicSpecification
                {
                    Name = typeof(TValue).Name,
                    NumPartitions = producer.NumPartitions
                }; 
                new KafkaTopicFactory(producerConfig).CreateTopic(topicSpecification);
            }
            
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<IProducer<TKey, TValue>>>();
                var kafkaProducer = new ProducerBuilder<TKey, TValue>(producerConfig)
                    .SetErrorHandler((x, e) => logger.LogError($"ErrorCode: {e.Code}, Reason: {e.Reason}"))
                    .SetLogHandler((x, e) => logger.LogInformation($"LogLevel: {e.Level}, LogName: {e.Name}, LogMessage: {e.Message}"))
                    .SetKeySerializer(new JsonSerializer<TKey>())
                    .SetValueSerializer(new JsonSerializer<TValue>())
                    .Build();

                return kafkaProducer;
            });
        }

        public static void AddKafkaConsumer<TKey, TValue>(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<IConsumer<TKey, TValue>>>();
                var kafkaConsumer = new KafkaConsumerFactory()
                    .CreateKafkaConsumer(configurationSection, logger);
                return kafkaConsumer;
            });
        }

        public static void AddMongoClient(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException($"Database configuration options not found: {nameof(configurationSection)}");
            }
            
            var databaseOptions = new DatabaseOptions();
            configurationSection.Bind(databaseOptions);

            if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
            {
                services.AddSingleton<IMongoClient>(new MongoClient(databaseOptions.ConnectionString));   
            }
        }
        
        public static void AddRepositories(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException($"Repositories configuration options not found: {nameof(configurationSection)}");
            }

            services.Configure<List<RepositoryOptions>>(configurationSection);
            services.AddSingleton<IRepository<MarketUpdateEntity>, MarketUpdateRepository>();
        }
        
        public static void AddRetryPolicy(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var retryPolicyOptions = new RetryPolicyOptions();
            configurationSection?.Bind(retryPolicyOptions);
            services.AddSingleton<IRetryPolicyOptions>(retryPolicyOptions);
        }
    }
}