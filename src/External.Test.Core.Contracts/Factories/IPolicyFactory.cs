using Microsoft.Extensions.Logging;
using Polly;

namespace External.Test.Contracts.Factories
{
    public interface IPolicyFactory
    {
        IAsyncPolicy CreateWaitAndRetryWithDecorrelatedJitterBackoff<TService>(int medianFirstRetryDelay,
            int retryCount, ILogger<TService> logger);
    }
}