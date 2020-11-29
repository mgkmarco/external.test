using System.Collections.Generic;

namespace External.Test.Contracts.Options
{
    public class KafkaOptions
    {
        public List<ProducerOptions> Producers { get; set; } = new List<ProducerOptions>();
    }
}