using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.src.Aplication.services;


namespace SmileTimeNET_API.src.Aplication.services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthServiceImpl(
        UserManager<ApplicationUser> userManager,
        EmailService emailService,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
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
        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var response = new AuthResponse();

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.Success = false;
                response.MessageResponse = "El email y la contraseña son requeridos";
                return response;
            }

            var user = await _userManager.FindByNameAsync(model.Email ?? string.Empty);
            if (user == null)
            {
                response.Success = false;
                response.MessageResponse = "Usuario no encontrado";
                return response;
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password ?? string.Empty);
            if (!result)
            {
                response.Success = false;
                response.MessageResponse = "Contraseña incorrecta";
                return response;
            }

            var tokenExpiration = DateTime.Now.AddDays(60);
            var token = await GenerateJwtTokenAsync(user, tokenExpiration);

            response.Success = true;
            response.Token = token;
            response.Email = user.Email ?? string.Empty;
            response.UserId = user.Id;
            response.TokenExpiration = tokenExpiration;
            response.MessageResponse = "Login exitoso";

            return response;
        }

        /// <summary>
        /// Realiza el registro de un nuevo usuario y devuelve un token JWT para login automático.
        /// </summary>
        /// <param name="model">El modelo de registro.</param>
        /// <returns>Un objeto que contiene el token JWT, el email, el ID del usuario y la fecha de expiración del token.</returns>
        /// <response code="200">El registro fue exitoso.</response>
        /// <response code="400">El email ya está registrado o hubo un error al registrar el usuario.</response>
        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            var response = new AuthResponse();

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.Success = false;
                response.MessageResponse = "El email y la contraseña son requeridos";
                return response;
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email ?? string.Empty);
            if (existingUser != null)
            {
                response.Success = false;
                response.MessageResponse = "El email ya está registrado, por favor inicie sesión";
                return response;
            }

            var user = new ApplicationUser
            {
                UserName = model.FullName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);

            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                var tokenExpiration = DateTime.Now.AddDays(60);
                var token = await GenerateJwtTokenAsync(user, tokenExpiration);

                response.Success = true;
                response.Token = token;
                response.Email = user.Email ?? string.Empty;
                response.UserId = user.Id;
                response.TokenExpiration = tokenExpiration;
                response.MessageResponse = "Registro exitoso";
                return response;
            }

            response.Success = false;
            response.MessageResponse = "Error al registrar el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return response;
        }
        
        public async Task<AuthResponse> ForgotPasswordAsync(string email)
{
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
        return new AuthResponse { Success = false, MessageResponse = "No se encontró un usuario con ese correo" };
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={token}&email={user.Email}";

    if (!string.IsNullOrEmpty(user.Email))
    {
        await _emailService.SendEmailAsync(user.Email, "Recuperación de contraseña", 
            $"Haz clic en el siguiente enlace para restablecer tu contraseña: {resetLink}");
    }
    else
    {
        return new AuthResponse { Success = false, MessageResponse = "El correo del usuario no está disponible." };
    }

    return new AuthResponse { Success = true, MessageResponse = "Se ha enviado un correo con instrucciones para recuperar la contraseña." };
}


        /// <summary>
        /// Genera un token JWT para un usuario específico con una fecha de expiración determinada. 
        /// </summary>
        /// <param name="user">El usuario para el cual se genera el token.</param>
        /// <param name="expires">La fecha de expiración del token.</param>
        /// <returns>El token JWT generado para el usuario .</returns> 
        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, DateTime expires)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            // informacion del perfil
            new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
            new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty),
        };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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