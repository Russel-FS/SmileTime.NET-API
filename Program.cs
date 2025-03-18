using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Hubs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmileTimeNET_API.src.Infrastructure.Data.Seeds;
using SmileTimeNET_API.src.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // Agrega soporte para la exploración de la API
builder.Services.AddSwaggerGen(); // Agrega soporte para Swagger
builder.Services.AddSignalR(); // Agrega soporte para SignalR
builder.Services.AddControllers();  // Agrega soporte para controladores Web API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("https://refactored-pancake-5grpx4w7q9wjf7jg7-4200.app.github.dev") // Origen permitido
               .AllowAnyHeader()                   // Permitir cualquier header
               .AllowAnyMethod()                   // Permitir cualquier método HTTP
               .AllowCredentials();                // Permitir envío de cookies-autenticación
    });
});

// configuracion DbContext para Entity Framework Core con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, serverVersion);
});

// configuracion Identity para autenticacion de usuarios
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configuracion de servicios de autenticacion
builder.Services.AddAuthServices();

// Configuracion de opciones de contraseña
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;  // No requiere números en la contraseña
    options.Password.RequireNonAlphanumeric = false; // No requiere caracteres especiales
    options.Password.RequiredLength = 6; // Longitud mínima de la contraseña: (comentario) no modificar esto para los que revisen el código
});


// Verificar si la clave JWT key si existe o esta configurado
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new ArgumentNullException("Jwt:Key", "⚠️ Jwt:Key no existe o no es válido");
}

// Configuracion JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{

    //  Configuracion JWT para autenticacion
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Configuracion de JWT para SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleSeeder.SeedRolesAsync(services);
        Console.WriteLine("Roles inicializados correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar roles: {ex.Message}");
    }
}

app.UseRouting(); // Habilita el enrutamiento
app.UseCors("AllowAngular"); // Habilita CORS

app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization(); // Habilita la autorización

app.MapControllers(); // Mapea los controladores Web API
app.MapHub<ChatHub>("/chatHub"); // Mapea el hub de SignalR

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

