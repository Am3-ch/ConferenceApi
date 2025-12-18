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
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(20)]
    public string Role { get; set; } = "Attendee"; // Attendee, Speaker, Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for refresh tokens
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
     public Speaker? Speaker { get; set; } // One user can be a speaker
    public ICollection<TalkRegistration> TalkRegistrations { get; set; } = new List<TalkRegistration>();
}