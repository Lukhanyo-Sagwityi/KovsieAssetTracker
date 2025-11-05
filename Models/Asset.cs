using System;
using System.ComponentModel.DataAnnotations;

namespace KovsieAssetTracker.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }
        public string? Name { get; set; } = "";

        public string? TagId { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Condition { get; set; } = "";
        public string? Location { get; set; } = "";
        public string? PhotoPath { get; set; }        // e.g., "/uploads/xyz.jpg"
        public bool IsVerified { get; set; } = false;
        public int? VerifiedById { get; set; }       // optional FK to User
        public DateTime? VerifiedAt { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string? VerificationStatus { get; set; } = "Pending"; // Pending, Accepted, Declined
        public ICollection<Verification>? Verifications { get; set; }
        public bool Verified { get; internal set; }
    }
}

