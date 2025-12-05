using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [Required]
    public required string PasswordHash { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}