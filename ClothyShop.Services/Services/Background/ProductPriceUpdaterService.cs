using ClothyShop.Business.Models.Hubs;
using ClothyShop.DAL.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace ClothyShop.Business.Services.Background
{
    public class ProductPriceUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<ProductHUB> _hubContext;
        private readonly Random _random = new();

        public ProductPriceUpdaterService(IServiceProvider serviceProvider, IHubContext<ProductHUB> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                var collection = database.GetCollection<Product>("Products");


                var products = await collection.Find(_ => true).ToListAsync(stoppingToken);

                foreach (var product in products)
                {
                    var randomValue = _random.NextDouble() * 1.9 + 0.1; // Generate random double between 0.1 and 2.0
                    var newPrice = Math.Round(product.Price + (decimal)randomValue, 2);

                    var update = Builders<Product>.Update.Set(p => p.Price, newPrice);

                    await collection.UpdateOneAsync(p => p.Id == product.Id, update, cancellationToken: stoppingToken);

                    product.Price = newPrice;
                }

                try
                {
                    await _hubContext.Clients.All.SendAsync("ProductsUpdated", products, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR error: {ex}");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
