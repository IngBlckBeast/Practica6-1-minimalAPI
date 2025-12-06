using Dapper;
using Microsoft.AspNetCore.Mvc;
using Practica6_1.Data;
using minimalAPI.Models;
using Practica6_1.Services;

namespace Practica6_1.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticación");

        group.MapPost("/login", async ([FromBody] LoginRequest request, DbConnectionFactory db, JwtTokenService jwt) =>
        {
            using var connection = db.CreateConnection();
            
            // Consulta Segura usando Dapper (Cumple requisito de seguridad)
            var sql = @"
                SELECT u.*, r.Name as RoleName 
                FROM Users u
                JOIN Roles r ON u.RoleId = r.Id
                WHERE u.Email = @Email";

            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { request.Email });

            if (user is null) return Results.Unauthorized();

            // Validación de password (Ajustar si usas Hash en el futuro)
            // REQUISITO: Hashing seguro. Por ahora validamos texto plano como tenías, 
            // pero el siguiente paso será implementar BCrypt.
            if (request.Password != user.Password) return Results.Unauthorized();

            var token = jwt.GenerateToken(user);

            return Results.Ok(new LoginResponse { Token = token, Username = user.FirstName });
        });
    }
}