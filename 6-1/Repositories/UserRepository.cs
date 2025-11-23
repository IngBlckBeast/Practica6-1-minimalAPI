using Dapper;
using Microsoft.Data.SqlClient;
using minimalAPI.Models;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        
        // Hacemos JOIN para traer el nombre del Rol directamente
        var sql = @"
            SELECT u.Id, u.FirstName, u.LastName, u.Email, u.PasswordHash, r.Name as RoleName
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Email = @Email";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }
}