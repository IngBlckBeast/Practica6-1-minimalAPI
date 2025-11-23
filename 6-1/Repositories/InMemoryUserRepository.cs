using minimalAPI.Models; // Usamos el MISMO modelo que SQL
using minimalAPI.Dtos;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public InMemoryUserRepository()
    {
        // Simulamos datos como si vinieran de la BD
        // OJO: Usamos la misma estructura de la clase User de Models
        _users.Add(new User 
        { 
            Id = 1, 
            FirstName = "Admin", 
            LastName = "Local", 
            Email = "admin@local.com", 
            PasswordHash = "admin123", // Contraseña simple para pruebas
            RoleName = "Admin" 
        });

        _users.Add(new User 
        { 
            Id = 2, 
            FirstName = "Daniel", 
            LastName = "Dev", 
            Email = "daniel@local.com", 
            PasswordHash = "user123", 
            RoleName = "User" 
        });
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        // Buscamos en la lista en memoria
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        
        // Retornamos como Task para cumplir la interfaz async
        return Task.FromResult(user); 
    }
}