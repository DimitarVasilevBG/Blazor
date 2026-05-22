using Microsoft.AspNetCore.SignalR;

namespace ClothyShop.Business.Models.Hubs
{
    public class ProductHUB : Hub
    {
        public async Task ReportProductIssue(string productName, string userEmail)
        {
            string alertMessage = $"User {userEmail} flagged an issue or request for: {productName}";

            // Send a notification message back down to ALL connected clients
            await Clients.All.SendAsync("ReceiveSystemAlert", alertMessage);
        }

        public async Task BroadcastProductUpdate()
        {
            await Clients.All.SendAsync("ProductsUpdated");
        }
    }
}
