using ClothyShop.DAL.Models;
using MongoDB.Driver;

namespace ClothyShop.DAL.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task EnsureSeededAsync()
        {
            var products = await GetAllAsync();
            if (products.Count == 0)
            {
                var exampleProducts = new List<Product>
            {
                new Product
                {
                    Name = "Classic White T-Shirt",
                    Description = "A comfy everyday shirt.",
                    Price = 19.99M,
                    Category = "Men"
                },
                new Product
                {
                    Name = "Floral Summer Dress",
                    Description = "Light, bright, and breezy.",
                    Price = 39.99M,
                    Category = "Women"
                }
            };

                foreach (var product in exampleProducts)
                {
                    await AddAsync(product);
                }
            }
        }
    }
}
