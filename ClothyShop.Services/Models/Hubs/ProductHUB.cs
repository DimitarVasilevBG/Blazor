using Microsoft.AspNetCore.SignalR;

namespace ClothyShop.Business.Models.Hubs
{
    public class ProductHUB : Hub
    {
        public async Task BroadcastProductUpdate()
        {
            await Clients.All.SendAsync("ProductsUpdated");
        }
    }
}
