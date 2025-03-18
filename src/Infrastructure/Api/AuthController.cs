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
using SmileTimeNET_API.src.Domain.Interfaces;



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
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación fallida en login");
                return BadRequest();
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro");
                return BadRequest();
            }
        }


    }
}