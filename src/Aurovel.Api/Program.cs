
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Aurovel.Api.Middleware;
using Aurovel.Application.Validation;
using Aurovel.Infrastructure.Mongo;
using Aurovel.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());

// Envs
string connString = Environment.GetEnvironmentVariable("MONGODB_URI") ?? "";
string dbName = Environment.GetEnvironmentVariable("MONGO_DB") ?? "CrmVeiculosAurovel";
string corsOriginsCsv = Environment.GetEnvironmentVariable("CORS_ORIGINS") ?? "";
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "missing-secret";
string issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "Aurovel";
string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "AurovelClients";

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(o => {
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSingleton<IValidator<MongoDB.Bson.BsonDocument>, ContactCreateValidator>(); // registered but our controllers ask concrete types
builder.Services.AddSingleton<ContactCreateValidator>();
builder.Services.AddSingleton<ContactUpdateValidator>();
builder.Services.AddSingleton<CompanyCreateValidator>();
builder.Services.AddSingleton<CompanyUpdateValidator>();

// Mongo
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Auth (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(opt => {
    var origins = corsOriginsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    opt.AddDefaultPolicy(p => {
        if (origins.Length > 0) p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        else p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "CRM API (.NET)", Version = "v1" });
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { { securityScheme, new string[] {} } });
});

// HealthChecks
builder.Services.AddHealthChecks().AddMongoDb(connString, name: "mongodb");

// Rate Limiting (equivalente simples ao express-rate-limit)
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter("fixed", options => {
    options.PermitLimit = 100;
    options.Window = TimeSpan.FromMinutes(1);
    options.QueueLimit = 0;
}));

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseSerilogRequestLogging();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

app.Run();
