using External.Test.Contracts.Options;
using External.Test.Data.Contracts.Entities;
using External.Test.Data.Contracts.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace External.Test.Data.Repositories
{
    public class MarketUpdateRepository : IRepository<MarketUpdateEntity>
    {
        private readonly IMongoClient _mongoClient;
        private readonly IOptionsMonitor<List<RepositoryOptions>> _repositoryOptions;
        private string _database;
        private string _collection;
        
        public MarketUpdateRepository(IMongoClient mongoClient, IOptionsMonitor<List<RepositoryOptions>> repositoryOptions)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            _repositoryOptions = repositoryOptions ?? throw new ArgumentNullException(nameof(repositoryOptions));
            _repositoryOptions.OnChange(x => SetDatabaseCollectionContext());
            SetDatabaseCollectionContext();
        }

        public IMongoCollection<MarketUpdateEntity> GetCollectionContext()
        {
            var collectionContext = _mongoClient
                .GetDatabase(_database)
                .GetCollection<MarketUpdateEntity>(_collection);

            return collectionContext;
        }

        private void SetDatabaseCollectionContext()
        {
            var repositoryOptions = _repositoryOptions.CurrentValue ??
                                    throw new ArgumentNullException(nameof(_repositoryOptions.CurrentValue));
            
            var repoOptions = repositoryOptions.FirstOrDefault(x => x.Name == nameof(MarketUpdateRepository));
            
            if (repoOptions == null)
            {
                throw new ArgumentNullException($"Repos configuration options not found: {nameof(repoOptions)}");
            }

            if (string.IsNullOrWhiteSpace(repoOptions.Database))
            {
                throw new ArgumentNullException($"Database name needs to be valid: {repoOptions.Database}");
            }
            
            if (string.IsNullOrWhiteSpace(repoOptions.Collection))
            {
                throw new ArgumentNullException($"Collection name needs to be valid: {repoOptions.Collection}");
            }
            
            _database = repoOptions.Database;
            _collection = repoOptions.Collection;
        }
    }
}