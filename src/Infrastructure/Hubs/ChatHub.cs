using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, ConnectedUser> ConnectedUsers = new ConcurrentDictionary<string, ConnectedUser>();

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = user?.FindFirst(ClaimTypes.Name)?.Value;

            if (userId != null && Context != null)
            {
                var connectedUser = new ConnectedUser
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId,
                    Username = username ?? "Anonymous",
                    ConnectedAt = DateTime.UtcNow,
                    IsOnline = true
                };

                ConnectedUsers.AddOrUpdate(userId, connectedUser, (key, oldValue) =>
                {
                    oldValue.ConnectionId = Context.ConnectionId;
                    oldValue.IsOnline = true;
                    return oldValue;
                });

                Console.WriteLine($"Usuario Conectado: {connectedUser.ToString()}");
                await Clients.Others.SendAsync("UserConnected", new
                {
                    UserId = userId,
                    Username = connectedUser.Username,
                    IsOnline = true
                });
            }
 
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && ConnectedUsers.TryGetValue(userId, out var user))
            {
                user.IsOnline = false;
                await Clients.Others.SendAsync("UserDisconnected", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Object message)
        {
            // test de mensaje 
            Console.WriteLine(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task SendPrivateMessage(string recipientUserId, string message)
        {
            var senderId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return;

            if (ConnectedUsers.TryGetValue(recipientUserId, out var recipient))
            {
                var senderUser = ConnectedUsers.GetValueOrDefault(senderId);
                var messageData = new
                {
                    SenderId = senderId,
                    SenderName = senderUser?.Username,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                await Clients.Client(recipient.ConnectionId ?? string.Empty).SendAsync("ReceivePrivateMessage", messageData);

                await Clients.Caller.SendAsync("ReceivePrivateMessage", messageData);
            }
        }

        public async Task UserTyping(Object userId, bool isTyping)
        {
            await Clients.All.SendAsync("UserTypingStatus", userId, isTyping);
        }

        //Usuarios en linea
        public async Task GetOnlineUsers()
        {
            var onlineUsers = ConnectedUsers.Values
                .Where(u => u.IsOnline)
                .Select(u => new { u.UserId, u.Username, u.IsOnline });

            await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
        }



    }
}
