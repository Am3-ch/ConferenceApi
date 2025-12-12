using System.ComponentModel.DataAnnotations;

public class Talk
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public required string Description { get; set; }
    
    [Required]
    public int SpeakerId { get; set; }
    
    [Required]
    public DateTime ScheduledAt { get; set; }
    
    public int DurationMinutes { get; set; } = 60;
    
    [MaxLength(100)]
    public string? Room { get; set; }
    
    [MaxLength(50)]
    public string Level { get; set; } = "Intermediate"; // Beginner, Intermediate, Advanced
    
    [MaxLength(100)]
    public string? Category { get; set; } // e.g., Web Development, AI/ML, DevOps
    
    public int MaxAttendees { get; set; } = 100;
    
    public int CurrentAttendees { get; set; } = 0;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Speaker Speaker { get; set; } = null!;
    public ICollection<TalkRegistration> Registrations { get; set; } = new List<TalkRegistration>();
}