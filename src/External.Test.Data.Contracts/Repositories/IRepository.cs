using MongoDB.Driver;

namespace External.Test.Data.Contracts.Repositories
{
    public interface IRepository<TEntity>
    {
        IMongoCollection<TEntity> GetCollectionContext();
    }
}