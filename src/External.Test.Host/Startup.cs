using AutoMapper;
using Confluent.Kafka;
using External.Test.Consumers;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using External.Test.Host.Extensions;
using External.Test.Producers;
using External.Test.Profiles;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace External.Test.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "External.Test.Core.WebApi", Version = "v1"});
            });
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddKafkaProducer<int, UpdateMarketCommand>(_configuration.GetSection("Kafka"));
            services.AddSingleton<IProducerService<int, UpdateMarketCommand>, MarketUpdateCommandProducer>();
            services.AddKafkaProducer<int, UpdateMarketSuccessEvent>(_configuration.GetSection("Kafka"));
            services.AddSingleton<IProducerService<int, UpdateMarketSuccessEvent>, MarketUpdateSuccessEventProducer>();
            services.AddKafkaProducer<int, UpdateMarketFailedEvent>(_configuration.GetSection("Kafka"));
            services.AddSingleton<IProducerService<int, UpdateMarketFailedEvent>, MarketUpdateFailedEventProducer>();
            services.AddKafkaConsumer<int, UpdateMarketCommand>(_configuration.GetSection("Kafka"));
            services.AddHostedService<MarketUpdateCommandConsumer>();
            services.AddMediatrPipeline();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMongoClient(_configuration.GetSection("Database"));
            services.AddRepositories(_configuration.GetSection("Database:Repositories"));
            services.AddHealthChecks()
                .AddMongoDb(_configuration["Database:ConnectionString"])
                .AddKafka(new ProducerConfig() {BootstrapServers = _configuration["Kafka:ConnectionString"]});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "External.Test.Core.WebApi v1"));
            app.UseHttpsRedirection();
            app
                .UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true
                });
                
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}