using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Hubs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmileTimeNET_API.src.Infrastructure.Data.Seeds;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using SmileTimeNET_API.src.Aplication.Mappings;
using SmileTimeNET_API.src.Infrastructure.DependencyInjection;
using SmileTimeNET_API.src.Aplication.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // Agrega soporte para la exploración de la API
builder.Services.AddSwaggerGen(options =>
{
    // Configuracion de Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmileTime API",
        Version = "v1"
    });
    // configuracion de JWT para Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

}); // Agrega soporte para Swagger

builder.Services.AddSignalR(); // Agrega soporte para SignalR
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });   // Agrega soporte para controladores Web API

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        // Configuracion de CORS para permitir el acceso desde el cliente Angular
        builder.WithOrigins("https://studious-journey-7vpgjpgqw664fwq6w-4200.app.github.dev", "http://localhost:4200") // Origen permitido

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
// Configuracion de servicios de carrusel
builder.Services.AddCarouselServices();
// Configuracion de servicios de chat
builder.Services.AddChatServices();
// Configuracion de servicios de administración
builder.Services.AddAdminServices(); // Ensure AdminServiceExtensions is defined and imported
// Configuracion de servicios de dentistas
builder.Services.AddDentistServices(); 
// Configuracion mapper 
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

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
builder.Services.AddScoped<EmailService>();
builder.Services.AddEmailServices();

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


