using RestApiAps.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// ===> Serviços
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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// ===> Configuração do Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    // Porta obrigatória para Render.com
    options.Listen(IPAddress.Any, 8080); // HTTP

    // HTTPS opcional se certificado estiver presente
    var base64Cert = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_BASE64");
    var certPassword = Environment.GetEnvironmentVariable("CERTIFICADO_PFX_SENHA") ?? string.Empty;

    if (!string.IsNullOrEmpty(base64Cert))
    {
        try
        {
            var certBytes = Convert.FromBase64String(base64Cert);
            var certificate = new X509Certificate2(certBytes, certPassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);

            options.Listen(IPAddress.Any, 8443, listenOptions =>
            {
                listenOptions.UseHttps(certificate);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar certificado: {ex.Message}");
        }
    }
});

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
