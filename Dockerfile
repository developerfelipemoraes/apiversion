FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copie csproj (cache)
COPY ./src/Aurovel.Api/Aurovel.Crm.Api.csproj ./src/Aurovel.Api/
# (se houver mais projetos, copie os .csproj também)

# copie o NuGet.config "limpo" antes do restore
COPY NuGet.Docker.config ./NuGet.config

# restore usando o config limpo
RUN dotnet restore ./src/Aurovel.Api/Aurovel.Crm.Api.csproj --configfile ./NuGet.config

# copie o restante do código
COPY ./src ./src

# publish usando o mesmo config
RUN dotnet publish ./src/Aurovel.Api/Aurovel.Crm.Api.csproj \
    -c Release -o /app /p:UseAppHost=false --no-restore --configfile ./NuGet.config
