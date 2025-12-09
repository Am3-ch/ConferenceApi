using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}