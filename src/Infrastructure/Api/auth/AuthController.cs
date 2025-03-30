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
using SmileTimeNET_API.src.Aplication.services;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.src.Domain.Models.auth;



namespace SmileTimeNET_API.rest
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        /// <summary>
        /// Inicia sesión de un usuario utilizando las credenciales proporcionadas y devuelve un token JWT.
        /// </summary>
        /// <param name="model">El modelo de inicio de sesión que contiene el email y la contraseña del usuario.</param>
        /// <returns>Un objeto que contiene el token JWT, el email, el ID del usuario y la fecha de expiración del token.</returns>
        /// <response code="200">El inicio de sesión fue exitoso.</response>
        /// <response code="400">La validación del modelo de inicio de sesión falló.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var response = await _authService.LoginAsync(model);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación fallida en login");
                return BadRequest(new { message = "Error de validación" });
            }
        }
        /// <summary>
        /// Registra un nuevo usuario y devuelve un token JWT para login automático.
        /// </summary>
        /// <param name="model">El modelo de registro.</param>
        /// <returns>Un objeto que contiene el token JWT, el email, el ID del usuario y la fecha de expiración del token.</returns>
        /// <response code="200">El registro fue exitoso.</response>
        /// <response code="400">El email ya está registrado o hubo un error al registrar el usuario.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var response = await _authService.RegisterAsync(model);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro");
                return BadRequest(new { Success = false, MessageResponse = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Envía un enlace de recuperación de contraseña al correo del usuario.
        /// </summary>
        /// <param name="model">El email del usuario.</param>
        /// <returns>Un mensaje indicando si el correo fue enviado con éxito.</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return BadRequest(new { Success = false, MessageResponse = "El email no puede estar vacío." });
                }

                var response = await _authService.ForgotPasswordAsync(model.Email);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la recuperación de contraseña");
                return BadRequest(new { Success = false, MessageResponse = "Error interno del servidor" });
            }
        }


    }
}