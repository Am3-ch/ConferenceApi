using System.ComponentModel.DataAnnotations;

public class DeleteAccountRequest
{
    [Required]
    public required string Password { get; set; }
}

