# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas os arquivos de projeto primeiro (melhora cache de build)
COPY *.csproj ./
RUN dotnet restore

# Copiar o restante dos arquivos e compilar
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar os arquivos da publica��o
COPY --from=build /app/out ./

# Configura a URL de escuta (obrigat�rio para Render.com)
ENV ASPNETCORE_URLS=http://+:8080

# Expose � apenas informativo no Render, mas bom para testes locais
EXPOSE 8080

# Entrypoint padr�o
ENTRYPOINT ["dotnet", "RestApiAps.dll"]
