using System.Collections.Generic;

namespace External.Test.Contracts.Options
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }
        public List<RepositoryOptions> Repositories { get; set; } = new List<RepositoryOptions>();
    }
}