using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(string userId);
    }
}