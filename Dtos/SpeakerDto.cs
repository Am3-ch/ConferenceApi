using System.ComponentModel.DataAnnotations;

public class CreateSpeakerDto
{
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
}

public class SpeakerResponseDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Bio { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? TwitterHandle { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int TotalTalks { get; set; }
}
