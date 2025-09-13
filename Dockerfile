# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copie o repo INTEIRO para /src (sem criar /src/src)
COPY . .

# restore/publish pelo csproj do API (refs relativos vão resolver)
RUN dotnet restore ./src/Aurovel.Api/Aurovel.Crm.Api.csproj
RUN dotnet publish ./src/Aurovel.Api/Aurovel.Crm.Api.csproj -c Release -o /app /p:UseAppHost=false

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:3001
EXPOSE 3001
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Aurovel.Crm.Api.dll"]   # ajuste se necessário
