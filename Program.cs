using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Authentication.BearerToken;
using SmileTimeNET_API.hubs;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddEndpointsApiExplorer(); // Agrega soporte para la exploración de la API
builder.Services.AddSwaggerGen(); // Agrega soporte para Swagger
builder.Services.AddSignalR(); // Agrega soporte para SignalR
builder.Services.AddControllers();  // Agrega soporte para controladores Web API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("https://studious-journey-7vpgjpgqw664fwq6w-4200.app.github.dev") // Origen permitido
               .AllowAnyHeader()                   // Permitir cualquier header
               .AllowAnyMethod()                   // Permitir cualquier método HTTP
               .AllowCredentials();                // Permitir envío de cookies-autenticación
    });
});
 
 

// Configuracion identity
 


var app = builder.Build();

 
app.UseRouting(); // Habilita el enrutamiento
app.UseCors("AllowAngular"); // Habilita CORS
 
app.MapControllers(); // Mapea los controladores Web API
app.MapHub<ChatHub>("/chatHub"); // Mapea el hub de SignalR

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
 
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
