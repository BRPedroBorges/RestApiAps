using Microsoft.EntityFrameworkCore;
using RestApiAps.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// ===> Certificado opcional via variáveis de ambiente
var base64Cert = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_BASE64");
var certPassword = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_SENHA") ?? string.Empty;

X509Certificate2? certificate = null;

if (!string.IsNullOrEmpty(base64Cert))
{
    try
    {
        var certBytes = Convert.FromBase64String(base64Cert);
        certificate = new X509Certificate2(certBytes, certPassword,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao carregar o certificado: {ex.Message}");
    }
}

var portEnv = Environment.GetEnvironmentVariable("PORT");
var port = string.IsNullOrEmpty(portEnv) ? 5000 : int.Parse(portEnv);

// Configuração do Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    if (certificate != null)
    {
        options.Listen(IPAddress.Any, port, listenOptions =>
        {
            listenOptions.UseHttps(certificate);
        });
    }
    else
    {
        options.Listen(IPAddress.Any, port); // Apenas HTTP
    }
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
