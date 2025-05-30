# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas os arquivos de projeto primeiro (melhor cache)
COPY *.csproj ./ 
RUN dotnet restore

# Copiar o restante e publicar
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia o build final
COPY --from=build /app/out ./

# Porta exposta apenas para documentação, Render ignora isso
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "RestApiAps.dll"]
