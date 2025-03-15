using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmileTimeNET_API.Models;


namespace SmileTimeNET_API.rest
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Console.WriteLine("Login");

            var user = await _userManager.FindByNameAsync(model.Email ?? string.Empty);
            if (user == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password ?? string.Empty);
            if (!result)
            {
                return BadRequest("Contraseña incorrecta");
            }


            var tokenExpiration = DateTime.Now.AddDays(60); // 60 días de expiración
            var token = GenerateJwtToken(user, tokenExpiration); // Generar token JWT

            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                UserId = user.Id,
                TokenExpiration = tokenExpiration
            };
            return Ok(response);
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email ?? string.Empty);
            if (existingUser != null)
            {
                return BadRequest("El email ya está registrado");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            // Agrega el nuevo usuario a la base de datos y  hashea la contraseña automáticamente
            var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);

            if (result.Succeeded)
            {

                var tokenExpiration = DateTime.Now.AddDays(60);  // 60 días de expiración
                var token = GenerateJwtToken(user, tokenExpiration);// Generar token JWT para login automático

                var response = new AuthResponse
                {
                    Token = token,
                    Email = user.Email ?? string.Empty,
                    UserId = user.Id,
                    TokenExpiration = tokenExpiration
                };
                return Ok(response);
            }

            return BadRequest(result.Errors);
        }

        // genera un token JWT con los datos del usuario y lo firma con la clave secreta
        private string GenerateJwtToken(ApplicationUser user, DateTime? expires)
        {

            var claims = new List<Claim>
        {
            // informacion del perfil
            new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
            new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty),
            // informacion del rol
            new Claim(ClaimTypes.Role, "User")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}