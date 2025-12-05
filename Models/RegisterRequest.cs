using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}