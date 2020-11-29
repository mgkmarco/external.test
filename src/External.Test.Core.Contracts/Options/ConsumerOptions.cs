using System.Collections.Generic;

namespace External.Test.Contracts.Options
{
    public class ConsumerOptions
    {
        public string MessageType { get; set; }
        public Dictionary<string, string> Configurations { get; set; }
    }
}