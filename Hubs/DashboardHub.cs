using Microsoft.AspNetCore.SignalR;

namespace SmileTimeNET_API.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task SendDashboardUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveDashboardUpdate", message);
        }
    }
}
