using Microsoft.EntityFrameworkCore;
using RestApiAps.Data;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Pega a porta do ambiente (Render define PORT)
var portEnv = Environment.GetEnvironmentVariable("PORT");
var port = string.IsNullOrEmpty(portEnv) ? 5000 : int.Parse(portEnv);

// Configuração do Kestrel para escutar em HTTP na porta do ambiente
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, port); // Apenas HTTP
});

// Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
