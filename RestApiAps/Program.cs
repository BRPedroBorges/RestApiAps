using RestApiAps.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Kestrel para aceitar conexões externas via HTTPS e HTTP
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // HTTPS com certificado (substitua o caminho e a senha abaixo)
    serverOptions.Listen(IPAddress.Parse("192.168.0.10"), 7188, listenOptions =>
    {
        listenOptions.UseHttps(@"C:\Users\Pedro Borges\source\repos\RestApiAps\certificado.pfx", "suaSenha");
    });

    // HTTP - usado apenas para testes locais com Unity
    serverOptions.Listen(IPAddress.Parse("192.168.0.10"), 5145);
});



// Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
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

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());



app.UseAuthorization();
app.MapControllers();
app.Run();
