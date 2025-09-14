# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copie todos os .csproj que o API referencia (cache do restore)
COPY ./src/Aurovel.Api/Aurovel.Crm.Api.csproj ./src/Aurovel.Api/
# COPY ./src/Aurovel.Domain/Aurovel.Domain.csproj ./src/Aurovel.Domain/
# COPY ./src/Aurovel.Application/Aurovel.Application.csproj ./src/Aurovel.Application/
# COPY ./src/Aurovel.Infrastructure/Aurovel.Infrastructure.csproj ./src/Aurovel.Infrastructure/

RUN dotnet restore ./src/Aurovel.Api/Aurovel.Crm.Api.csproj

# copie o restante do código
COPY ./src ./src

# publish (sem restaurar de novo)
RUN dotnet publish ./src/Aurovel.Api/Aurovel.Crm.Api.csproj \
    -c Release -o /app /p:UseAppHost=false --no-restore

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# instala curl para healthcheck HTTP (aspnet runtime slim não vem com curl/wget)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:3001
EXPOSE 3001

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Aurovel.Crm.Api.dll"]
