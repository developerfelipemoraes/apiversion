
# CRM API Migration (.NET 8) — Paridade com Node.js

**Base paths**:  
- `/api/auth` — POST `/login`, POST `/refresh`, GET `/me`  
- `/api/contacts` — POST `/`, GET `/`, GET `/stats`, GET `/:id`, GET `/cpf/:cpf`, PUT `/:id`, DELETE `/:id`, POST `/:contactId/companies/:companyId`, DELETE `/:contactId/companies/:companyId`  
- `/api/companies` — POST `/`, GET `/`, GET `/stats`, GET `/:id`, GET `/cnpj/:cnpj`, PUT `/:id`, DELETE `/:id`  
- `/api/matching` — GET `/company/:companyId/contacts`, GET `/contact/:contactId/companies`, GET `/best-matches`, POST `/links`, DELETE `/links/:companyId/:contactId`

## Variáveis de ambiente
- `MONGODB_URI` (Atlas) — **não versione**
- `MONGO_DB` (padrão: CrmVeiculosAurovel)
- `JWT_SECRET`
- `CORS_ORIGINS` (CSV, ex: `http://localhost:3000,http://localhost:5173`)
- `ASPNETCORE_ENVIRONMENT` (Development)
- `ASPNETCORE_URLS` (`http://+:3001`)

## Rodando
```bash
dotnet restore ./src/Aurovel.Api/Aurovel.Api.csproj
dotnet run --project ./src/Aurovel.Api/Aurovel.Api.csproj
# Swagger: /swagger
```

## Docker
```bash
docker build -t aurovel-api-net .
docker compose up -d
```

> **TODO**: Ajustar regras exatas de matching conforme a API Node (há trechos ellipsis no fonte).
