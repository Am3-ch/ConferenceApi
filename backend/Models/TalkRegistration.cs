public class TalkRegistration
{
    public int Id { get; set; }
    
    public int TalkId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    public bool Attended { get; set; } = false;
    
    public DateTime? AttendedAt { get; set; }
    
    // Navigation properties
    public Talk Talk { get; set; } = null!;
    public User User { get; set; } = null!;
}
