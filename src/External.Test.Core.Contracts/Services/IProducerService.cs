using System;
using System.Threading.Tasks;

namespace External.Test.Contracts.Services
{
    public interface IProducerService<in TKey, in TMessage> : IDisposable
    {
        Task ProduceAsync(TKey key, TMessage message);
    }
}