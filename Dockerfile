
# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src ./src
RUN dotnet restore ./src/Aurovel.Api/Aurovel.Api.csproj
RUN dotnet publish ./src/Aurovel.Api/Aurovel.Api.csproj -c Release -o /app

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:3001
EXPOSE 3001
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aurovel.Api.dll"]
