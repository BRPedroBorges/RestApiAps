using RestApiAps.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// ===> L� o certificado a partir da vari�vel de ambiente CERTIFICADO_PFX_BASE64
var base64Cert = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_BASE64");
var certPassword = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_SENHA") ?? string.Empty;

X509Certificate2? certificate = null;

if (!string.IsNullOrEmpty(base64Cert))
{
    var certBytes = Convert.FromBase64String(base64Cert);
    certificate = new X509Certificate2(certBytes, certPassword,
        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
}

// ===> L� a porta do ambiente Render (obrigat�rio l�)
var portEnv = Environment.GetEnvironmentVariable("PORT");
var port = string.IsNullOrEmpty(portEnv) ? 5000 : int.Parse(portEnv); // fallback local

// Configura��o do Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    if (certificate != null)
    {
        serverOptions.Listen(IPAddress.Any, port, listenOptions =>
        {
            listenOptions.UseHttps(certificate);
        });
    }
    else
    {
        serverOptions.Listen(IPAddress.Any, port);
    }
});

// Servi�os
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

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
