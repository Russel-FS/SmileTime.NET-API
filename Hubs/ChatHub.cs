using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmileTimeNET_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> UserConnections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                UserConnections[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && UserConnections.ContainsKey(userId))
            {
                UserConnections.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string recipientUserId, string message)
        {
            if (UserConnections.TryGetValue(recipientUserId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }
}
