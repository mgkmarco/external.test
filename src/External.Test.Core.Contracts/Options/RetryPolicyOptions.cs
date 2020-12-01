using System;

namespace External.Test.Contracts.Options
{
    public class RetryPolicyOptions : IRetryPolicyOptions
    {
        public int MedianFirstRetryDelayInSeconds { get; set; } = 5;
        public int RetryCount { get; set; } = 5;
    }
    
    public interface IRetryPolicyOptions
    {
        public int MedianFirstRetryDelayInSeconds { get; set; }
        public int RetryCount { get; set; }
    }
}