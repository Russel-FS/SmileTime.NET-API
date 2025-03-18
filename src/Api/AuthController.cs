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

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AuthController"/>.
        /// </summary>
        /// <param name="userManager">El administrador de usuarios.</param>
        /// <param name="configuration"> La configuración de la aplicación.</param>
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Realiza el inicio de sesión de un usuario.
        /// </summary>
        /// <param name="model">El modelo de inicio de sesión.</param>
        /// <returns>Un objeto que contiene el token JWT, el email, el ID del usuario y la fecha de expiración del token.</returns>
        /// <response code="200">El inicio de sesión fue exitoso.</response>
        /// <response code="400">El usuario no existe o la contraseña es incorrecta.</response>
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
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="model">El modelo de registro que contiene el email y la contraseña del usuario.</param>
        /// <returns>Una respuesta que indica si el registro fue exitoso o un error si no lo fue.</returns>
        /// <response code="200">El registro fue exitoso y retorna un token JWT.</response>
        /// <response code="400">El email ya está registrado o hubo errores al crear el usuario.</response>
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

        /// <summary>
        /// Genera un token JWT para un usuario específico con una fecha de expiración determinada. 
        /// </summary>
        /// <param name="user">El usuario para el cual se genera el token.</param>
        /// <param name="expires">La fecha de expiración del token.</param>
        /// <returns>El token JWT generado para el usuario .</returns> 
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