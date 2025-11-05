using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty; // Initialize with empty string

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty; // Initialize with empty string

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}