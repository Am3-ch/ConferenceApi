using System.ComponentModel.DataAnnotations;

public class CreateTalkDto
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public required string Description { get; set; }
    
    [Required]
    public DateTime ScheduledAt { get; set; }
    
    public int DurationMinutes { get; set; } = 60;
    
    [MaxLength(100)]
    public string? Room { get; set; }
    
    [MaxLength(50)]
    public string Level { get; set; } = "Intermediate";
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    public int MaxAttendees { get; set; } = 100;
}

public class UpdateTalkDto
{
    [MaxLength(200)]
    public string? Title { get; set; }
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public DateTime? ScheduledAt { get; set; }
    
    public int? DurationMinutes { get; set; }
    
    [MaxLength(100)]
    public string? Room { get; set; }
    
    [MaxLength(50)]
    public string? Level { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    public int? MaxAttendees { get; set; }
    
    [MaxLength(20)]
    public string? Status { get; set; }
}

public class TalkResponseDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Room { get; set; }
    public string Level { get; set; } = "Intermediate";
    public string? Category { get; set; }
    public int MaxAttendees { get; set; }
    public int CurrentAttendees { get; set; }
    public string Status { get; set; } = "Scheduled";
    public SpeakerResponseDto Speaker { get; set; } = null!;
    public bool IsUserRegistered { get; set; }
}
