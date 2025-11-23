using minimalAPI.Models; 

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
}