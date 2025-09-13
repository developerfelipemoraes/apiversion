# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie apenas os .csproj primeiro (melhora cache do restore)
COPY ./src/Aurovel.Api/Aurovel.Crm.Api.csproj ./src/Aurovel.Api/
# (se houver outros projetos referenciados, copie os .csproj deles aqui também)

# Restaura as dependências
RUN dotnet restore ./src/Aurovel.Api/Aurovel.Crm.Api.csproj

# Agora copie o restante do código
COPY ./src ./src

# Publica o projeto (apontando para o .csproj)
RUN dotnet publish ./src/Aurovel.Api/Aurovel.Crm.Api.csproj -c Release -o /app /p:UseAppHost=false

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Porta de escuta no container
ENV ASPNETCORE_URLS=http://+:3001
EXPOSE 3001

# Copia artefatos publicados
COPY --from=build /app .

# Se o assembly final for Aurovel.Crm.Api.dll (o padrão do projeto)
ENTRYPOINT ["dotnet", "Aurovel.Crm.Api.dll"]
