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
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AuthController"/>.
        /// </summary>
        /// <param name="userManager">El administrador de usuarios.</param>
        /// <param name="roleManager">El administrador de roles.</param>
        /// <param name="configuration"> La configuración de la aplicación.</param>
        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
             IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
            var response = new AuthResponse();

            // Verificar si el email y la contraseña son nulos o vacíos
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.MessageResponse = "El email y la contraseña son requeridos";
                return BadRequest(response);
            }

            var user = await _userManager.FindByNameAsync(model.Email ?? string.Empty);
            if (user == null)
            {
                response.MessageResponse = "Usuario no encontrado";
                return BadRequest(response);
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password ?? string.Empty);
            if (!result)
            {
                response.MessageResponse = "Contraseña incorrecta";
                return BadRequest(response);
            }


            var tokenExpiration = DateTime.Now.AddDays(60); // 60 días de expiración
            var token = GenerateJwtToken(user, tokenExpiration); // Generar token JWT 

            response.Token = token;
            response.Email = user.Email ?? string.Empty;
            response.UserId = user.Id;
            response.TokenExpiration = tokenExpiration;

            return Ok(response);
        }
        /// <summary>
        /// Realiza el registro de un nuevo usuario y devuelve un token JWT para login automático.
        /// </summary>
        /// <param name="model">El modelo de registro.</param>
        /// <returns>Un objeto que contiene el token JWT, el email, el ID del usuario y la fecha de expiración del token.</returns>
        /// <response code="200">El registro fue exitoso.</response>
        /// <response code="400">El email ya está registrado o hubo un error al registrar el usuario.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = new AuthResponse();

            // Verificar si el email y la contraseña son nulos o vacíos
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.MessageResponse = "El email y la contraseña son requeridos";
                return BadRequest(response);
            }

            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email ?? string.Empty);
            if (existingUser != null)
            {
                response.MessageResponse = "El email ya está registrado, por favor inicie sesión";
                return BadRequest(response);
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

                response.Token = token; // Token JWT
                response.Email = user.Email ?? string.Empty; // Email del usuario
                response.UserId = user.Id; // ID del usuario
                response.TokenExpiration = tokenExpiration; // Fecha de expiración del token
                return Ok(response); // Retorna el token JWT
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