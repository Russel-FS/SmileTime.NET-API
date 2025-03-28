using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;
using SmileTimeNET_API.src.Aplication.DTOs.signalR;
using SmileTimeNET_API.src.Domain.Models.signalR;

namespace SmileTimeNET_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, ConnectedUser> ConnectedUsers = new ConcurrentDictionary<string, ConnectedUser>();

        /// <summary>
        /// Se ejecuta cuando un cliente se conecta al hub.
        /// </summary>
        /// <remarks>
        /// Se obtiene el ID de usuario y el nombre de usuario del <see cref="System.Security.Claims.ClaimsPrincipal"/>
        /// y se utilizan para crear una nueva instancia de <see cref="ConnectedUser"/>.
        /// Esta instancia se agrega al diccionario <see cref="ConnectedUsers"/>.
        /// Se envía un mensaje a todos los clientes conectados para informarles que un nuevo usuario se ha conectado.
        /// </remarks>
        /// <returns>Una tarea que se completa cuando se ha conectado</returns>
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

        /// <summary>
        /// Cuando se desconecta un usuario, se marca como desconectado, se notifica a los demas usuarios
        /// y se remueve de la lista de conectados.
        /// </summary>
        /// <param name="exception">La excepcion que se produjo al desconectar</param>
        /// <returns>Una tarea que se completa cuando se ha desconectado</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null && ConnectedUsers.TryGetValue(userId, out var user_))
            {
                // marcar usuario como desconectado
                user_.IsOnline = false;
                //  log
                Console.WriteLine($"Usuario Desconectado: {user_.ToString()}");
                // notificar a los demas usuarios que se desconecto
                await Clients.Others.SendAsync("UserDisconnected", userId);
                // remover usuario de la lista de conectados
                ConnectedUsers.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Envia un mensaje a todos los clientes conectados al hub.
        /// </summary>
        /// <param name="message">El mensaje que se va a enviar a todos los clientes.</param>
        /// <returns>Una tarea que representa la operacion asincrona de envio de mensaje.</returns>
        public async Task SendMessage(Object message)
        {
            // test de mensaje 
            Console.WriteLine(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        /// <summary>
        /// Envia un mensaje privado a un usuario especifico.
        /// </summary>
        /// <param name="recipientUserId">El ID del usuario al que se va a enviar el mensaje privado.</param>
        /// <param name="message">El mensaje que se va a enviar.</param>
        /// <returns>Una tarea que representa la operacion asincrona de envio de mensaje.</returns>
        public async Task SendPrivateMessage(PrivateMessageDTO message)
        {
            Console.WriteLine($"Mensaje Recibido: {JsonSerializer.Serialize(message)}");
            var recipientUserId = message.RecipientId;
            var senderId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return;

            if (ConnectedUsers.TryGetValue(recipientUserId, out var recipient))
            {
                await Clients.Client(recipient.ConnectionId ?? string.Empty)
                       .SendAsync("ReceivePrivateMessage", message);
                await Clients.Caller.SendAsync("ReceivePrivateMessage", message);
            }
        }

        /// <summary>
        /// Notifica a todos los clientes conectados que un usuario esta o no esta escribiendo.
        /// </summary>
        /// <param name="userId">El ID del usuario que esta escribiendo.</param>
        /// <param name="isTyping">Indica si el usuario esta escribiendo o no.</param>
        /// <returns>Una tarea que se completa cuando se ha notificado a todos los clientes.</returns>
        public async Task UserTyping(TypingStatus isTyping)
        {
            await Clients.All.SendAsync("UserTypingStatus", isTyping);
        }


        /// <summary>
        /// Obtiene la lista de usuarios conectados actualmente.
        /// </summary>
        /// <returns>
        /// Una tarea que se completa cuando se ha obtenido la lista de usuarios conectados.
        /// </returns>
        public async Task GetOnlineUsers()
        {
            var onlineUsers = ConnectedUsers.Values
                .Where(u => u.IsOnline)
                .Select(u => new OnlineUserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    IsOnline = u.IsOnline
                });



            await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
        }



    }
}
