using External.Test.Contracts.Factories;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System;

namespace External.Test.Factories
{
    public class PolicyFactory : IPolicyFactory
    {
        public IAsyncPolicy CreateWaitAndRetryWithDecorrelatedJitterBackoff<TService>(int medianFirstRetryDelay, int retryCount, ILogger<TService> logger)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(medianFirstRetryDelay),
                retryCount: retryCount);
            
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(delay,
                    (exception, span, retCount, context) =>
                    {
                        logger.LogError(exception,
                            $"Failed. Retry count: {retCount}, exception message: {exception.Message}");
                    });

            return retryPolicy;
        }
    }
}