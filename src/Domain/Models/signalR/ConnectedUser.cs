using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.Models
{
    public class ConnectedUser
    {
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; }
        public string? Username { get; set; }
        public DateTime ConnectedAt { get; set; }
        public bool IsOnline { get; set; }

        public override string ToString()
        {
            return $"""
           ╔════════════════════════════════
           ║ Usuario
           ╠════════════════════════════════
           ║ 🆔 ID: {UserId}
           ║ 👤 Usuario: {Username}
           ║ 🔌 Conexión: {ConnectionId}
           ║ 🕒 Conectado: {ConnectedAt:dd/MM/yyyy HH:mm:ss}
           ║ 📡 Estado: {(IsOnline ? "✅ En línea" : "❌ Desconectado")}
           ╚════════════════════════════════
           """;
        }
    }
}