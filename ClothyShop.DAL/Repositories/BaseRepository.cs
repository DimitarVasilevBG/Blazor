using ClothyShop.DAL.Attributes;
using ClothyShop.DAL.Models;
using ClothyShop.DAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClothyShop.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public BaseRepository(IMongoDatabase database)
        {
            var collectionName = GetCollectionName(); //The entity must have collection name attribute
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        private string GetCollectionName()
        {
            var attr = (CollectionNameAttribute?)Attribute.GetCustomAttribute(typeof(TEntity), typeof(CollectionNameAttribute));
            if (attr == null)
                throw new InvalidOperationException($"CollectionName attribute is not defined on {typeof(TEntity).Name}.");

            return attr.Name;
        }

        public Task AddAsync(TEntity entity)
        {
            return _collection.InsertOneAsync(entity);
        }

        public Task DeleteAsync(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            return _collection.DeleteOneAsync(filter);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<TEntity> GetByIdAsync(string id)
        {
            return _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidOperationException("Entity Id is null or empty.");

            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

            return _collection.ReplaceOneAsync(filter, entity);
        }
    }
}
