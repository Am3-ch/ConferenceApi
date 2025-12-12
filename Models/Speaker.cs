using System.ComponentModel.DataAnnotations;

public class Speaker
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string FullName { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public required string Bio { get; set; }
    
    [MaxLength(200)]
    public string? Company { get; set; }
    
    [MaxLength(100)]
    public string? JobTitle { get; set; }
    
    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }
    
    [MaxLength(200)]
    public string? TwitterHandle { get; set; }
    
    [MaxLength(200)]
    public string? LinkedInUrl { get; set; }
    
    [MaxLength(200)]
    public string? WebsiteUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Talk> Talks { get; set; } = new List<Talk>();
}