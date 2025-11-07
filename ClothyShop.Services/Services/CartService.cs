using ClothyShop.DAL.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace ClothyShop.Business.Services
{
    public class CartService
    {
        private readonly IDatabase _redis;

        public CartService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public event Action OnProductsChange; // event to notify UI

        private string GetCartKey(string userId) => $"cart:{userId}";

        public async Task<List<CartProductItem>> GetCartAsync(string userId)
        {
            var key = GetCartKey(userId);
            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty) return new List<CartProductItem>();

            return JsonSerializer.Deserialize<List<CartProductItem>>(value);
        }

        public async Task AddToCartAsync(string userId, string productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartProductItem { ProductId = productId, Quantity = quantity });

            await _redis.StringSetAsync(GetCartKey(userId), JsonSerializer.Serialize(cart));
        }

        public async Task RemoveFromCartAsync(string userId, string productId)
        {
            var cart = await GetCartAsync(userId);
            cart.RemoveAll(c => c.ProductId == productId);
            await _redis.StringSetAsync(GetCartKey(userId), JsonSerializer.Serialize(cart));
        }

        public async Task ClearCartAsync(string userId)
        {
            await _redis.KeyDeleteAsync(GetCartKey(userId));
        }
    }
}
