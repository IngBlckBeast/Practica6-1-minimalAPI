using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Microsoft.OpenApi.Models;
using Microsoft.Data.SqlClient;
using minimalAPI.Dtos;
using minimalAPI.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- Swagger ---
builder.Services.AddScoped<UserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Practica 6 - JWT + Roles",
        Version = "v1"
    });

    // Config para que Swagger permita enviar el JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Escribe: Bearer {tu token JWT}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// --- HealthChecks basicos ---
builder.Services.AddHealthChecks();

// 1) Auth JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// 2) Roles / Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});

// 3) Dependencias para usuarios en memoria
//builder.Services.AddSingleton<PasswordHasher<User>>();
//builder.Services.AddSingleton<UserStore>();

var app = builder.Build();

// --- Swagger UI ---
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- Prometheus metrics ---
app.UseHttpMetrics();   // mide las requests HTTP

app.UseAuthentication();
app.UseAuthorization();

// 4) Endpoint de login: POST /auth/login
app.MapPost("/auth/login", async (LoginDto login, UserRepository userRepo, IConfiguration config) =>
{
    // 1. Buscar usuario en Base de Datos
    var user = await userRepo.GetUserByEmailAsync(login.Email);

    if (user is null) return Results.Unauthorized();

    // 2. Verificar Contraseña
    // NOTA: Como en SQL pusimos textos falsos como 'HASH_DE_USER', 
    // para esta prueba simple validaremos texto plano O el hash si ya implementaste BCrypt.
    // Si en tu práctica 6-1 usabas BCrypt.Verify, úsalo. 
    // Si no, para validar que conecta, haremos una comparación simple:
    
    // --> AJUSTA ESTA LÍNEA SEGÚN TU LÓGICA DE HASH DE LA SESIÓN 6 <--
    // Ejemplo simple (inseguro, solo para testear conexión):
    if (user.PasswordHash != login.Password) 
    {
        // Si tienes BCrypt instalado sería: 
        // if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash)) return Results.Unauthorized();
        
        // Por ahora, para que funcione el test rápido, retornaremos Unauthorized si no coinciden
        return Results.Unauthorized(); 
    }

    // 3. Generar Token (Usando los datos reales de la BD)
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.RoleName), // Aquí viene "Admin" o "User" desde SQL
        new Claim("NombreCompleto", $"{user.FirstName} {user.LastName}") // Dato extra
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});

// 5) Endpoint publico
app.MapGet("/public/ping", () => Results.Ok("pong"))
    .AllowAnonymous()
    .WithTags("Public");

// 6) Endpoint protegido (cualquier usuario autenticado)
app.MapGet("/api/me", (ClaimsPrincipal user) =>
{
    var username = user.Identity?.Name;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

    return Results.Ok(new
    {
        username,
        roles
    });
})
    .RequireAuthorization()
    .WithTags("User");

// 7) Endpoint protegido por rol Admin
app.MapGet("/admin/secret", () =>
{
    return Results.Ok("Solo admins pueden ver esto.");
})
    .RequireAuthorization("AdminOnly")
    .WithTags("Admin");

// 8) Endpoint para ver entorno
app.MapGet("/environment", (IHostEnvironment env, IConfiguration cfg) =>
{
    return Results.Ok(new
    {
        Environment = env.EnvironmentName,
        ApplicationName = env.ApplicationName,
        MachineName = Environment.MachineName
    });
})
    .AllowAnonymous()
    .WithTags("Info");

// 9) HealthChecks
app.MapHealthChecks("/health")
   .WithTags("Health");

// 10) Endpoint de m�tricas de Prometheus
app.MapMetrics("/metrics")
   .WithTags("Metrics");

app.Run();


// ======= TIPOS Y SERVICIOS (DESPUES de app.Run) =======

//record LoginRequest(string Username, string Password);
//record LoginResponse(string AccessToken);

//public record User(Guid Id, string Username, string PasswordHash, string[] Roles);

/*public class UserStore
{
    private readonly List<User> _users = new();
    private readonly PasswordHasher<User> _passwordHasher;

    public UserStore(PasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
        Seed();
    }

    private void Seed()
    {
        // usuario admin
        AddUser("admin", "Admin123!", new[] { "Admin", "User" });

        // usuario normal
        AddUser("Daniel", "User123!", new[] { "User" });
    }

    private void AddUser(string username, string plainPassword, string[] roles)
    {
        var user = new User(Guid.NewGuid(), username, "", roles);
        var hash = _passwordHasher.HashPassword(user, plainPassword);

        _users.Add(user with { PasswordHash = hash });
    }

    public User? FindByUsername(string username) =>
        _users.SingleOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
}

public static class JwtTokenService
{
    public static string GenerateJwtToken(User user, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}*/
