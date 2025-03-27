using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models;


namespace SmileTimeNET_API.src.Domain.Interfaces
{
     public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> RegisterAsync(RegisterModel model);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user, DateTime expires);
    }
}