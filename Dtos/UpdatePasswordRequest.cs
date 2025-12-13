using System.ComponentModel.DataAnnotations;

public class UpdatePasswordRequest
{
    [Required]
    public required string CurrentPassword { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string NewPassword { get; set; }
    
    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public required string ConfirmNewPassword { get; set; }
}

