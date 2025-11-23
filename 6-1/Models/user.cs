namespace minimalAPI.Models; // Ajusta el namespace al nombre de tu proyecto

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // --- NUEVO: La contraseña legible ---
    public string Password { get; set; } = string.Empty; 
    
    public string PasswordHash { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}