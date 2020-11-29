using System.Collections.Generic;

namespace External.Test.Contracts.Options
{
    public class ProducerOptions
    {
        public string MessageType { get; set; }
        public bool EnableTopicCreation { get; set; }
        public int NumPartitions { get; set; }
        public Dictionary<string, string> Configurations { get; set; }
    }
}