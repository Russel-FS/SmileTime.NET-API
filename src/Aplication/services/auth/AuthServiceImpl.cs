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

namespace SmileTimeNET_API.src.Aplication.services
{
    public class AuthServiceImpl : IAuthService
    {
        private static readonly string[] AllowedRoles = { "Admin", "User", "Dentist" };
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthServiceImpl(
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
        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var response = new AuthResponse();

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.Success = false;
                response.MessageResponse = "El email y la contraseña son requeridos";
                return response;
            }

            var user = await _userManager.FindByEmailAsync(model.Email ?? string.Empty);
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
            // Obtener roles del usuario
            var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

            var tokenExpiration = DateTime.Now.AddDays(60);
            var token = await GenerateJwtTokenAsync(user, tokenExpiration);

            response.Success = true;
            response.Token = token;
            response.Email = user.Email ?? string.Empty;
            response.UserId = user.Id;
            response.Roles = userRoles;
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

            // Validación inicial de datos
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                response.Success = false;
                response.MessageResponse = "El email y la contraseña son requeridos";
                return response;
            }

            // Verificar si el usuario ya existe
            var existingEmail = await _userManager.FindByEmailAsync(model.Email ?? string.Empty);
            if (existingEmail != null)
            {
                response.Success = false;
                response.MessageResponse = "El email ya está registrado, por favor inicie sesión";
                return response;
            }

            var existingUser = await _userManager.FindByNameAsync(model.FullName ?? string.Empty);
            if (existingUser != null)
            {
                response.Success = false;
                response.MessageResponse = $"El nombre de usuario '{model.FullName}' ya está en uso";
                return response;
            }

            // Crear nuevo usuario
            var user = new ApplicationUser
            {
                UserName = model.FullName,
                Email = model.Email,
            };

            // Crear usuario en la base de datos
            var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);

            if (result.Succeeded)
            {
                try
                {
                    // Determinar el rol a asignar
                    string roleToAssign = !string.IsNullOrEmpty(model.Role)
                        && AllowedRoles.Contains(model.Role) ? model.Role : "User";

                    // Crear el rol si no existe
                    if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    {
                        var roleCreateResult = await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
                        if (!roleCreateResult.Succeeded)
                        {
                            response.Success = false;
                            response.MessageResponse = "Error al crear el rol: " +
                                string.Join(", ", roleCreateResult.Errors.Select(e => e.Description));
                            return response;
                        }
                    }

                    // Asignar el rol al usuario
                    var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);
                    if (!roleResult.Succeeded)
                    {
                        response.Success = false;
                        response.MessageResponse = "Error al asignar el rol: " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        return response;
                    }

                    var userRoles = (await _userManager.GetRolesAsync(user)).ToList();
                    // Generar token JWT
                    var tokenExpiration = DateTime.Now.AddDays(60);
                    var token = await GenerateJwtTokenAsync(user, tokenExpiration);

                    // Preparar respuesta exitosa
                    response.Success = true;
                    response.Token = token;
                    response.Email = user.Email ?? string.Empty;
                    response.UserId = user.Id;
                    response.Roles = userRoles;
                    response.TokenExpiration = tokenExpiration;
                    response.MessageResponse = "Registro exitoso";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.MessageResponse = "Error durante el proceso de registro: " + ex.Message;
                    // Eliminar el usuario si falló la asignación de rol
                    await _userManager.DeleteAsync(user);
                    return response;
                }
            }
            response.Success = false;
            response.MessageResponse = "Error al registrar el usuario: " +
       string.Join(", ", result.Errors.Select(e => e.Description));
            return response;
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