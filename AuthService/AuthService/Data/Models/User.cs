namespace CRMSolution.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } 
    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Manager;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid RefreshToken { get; set; } = Guid.NewGuid();
    public DateTime RefreshTokenExpiration { get; set; } = DateTime.UtcNow.AddDays(7);
    
}

public enum UserRole
{
    Admin,
    Manager
}