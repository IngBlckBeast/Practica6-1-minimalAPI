using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Practica6_1.Data;
using Practica6_1.Endpoints;
using Practica6_1.Services;
using Prometheus; // Para las métricas que ya tenías

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS (Inyección de Dependencias) ---

// Base de Datos y Servicios
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<JwtTokenService>();

// Swagger (Documentación)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticación JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

// Health Checks (Requisito del proyecto)
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- 2. PIPELINE HTTP (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Métricas de Prometheus (Ya lo tenías, lo mantenemos)
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

// --- 3. ENDPOINTS ---

// Mapeamos los endpoints desde sus archivos separados
app.MapAuthEndpoints();

// Endpoints de sistema
app.MapHealthChecks("/health");
app.MapMetrics("/metrics");

app.Run();