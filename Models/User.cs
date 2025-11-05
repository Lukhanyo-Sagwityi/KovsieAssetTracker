using System.ComponentModel.DataAnnotations;

namespace KovsieAssetTracker.Models
{
    public class User
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
    }
}
