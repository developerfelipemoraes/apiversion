# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./src/Aurovel.Api/Aurovel.Crm.Api.csproj" 
RUN dotnet publish "./src/Aurovel.Api/Aurovel.Crm.Api.csproj" -c Release -o /app /p:UseAppHost=false --no-restore

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aurovel.Crm.Api.dll"]