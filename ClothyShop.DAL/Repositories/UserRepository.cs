using ClothyShop.DAL.Models;
using MongoDB.Driver;

namespace ClothyShop.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IMongoDatabase database) : base(database) { }

        public Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
