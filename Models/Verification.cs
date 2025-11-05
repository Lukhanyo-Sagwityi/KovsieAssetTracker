using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KovsieAssetTracker.Models
{
    public class Verification
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int VerificationId { get; set; }
        [ForeignKey("Asset")]
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }                 // optional reference to user table
        public User? Agent { get; set; }
        public string AgentName { get; set; } = string.Empty; // used by view
        public string VerificationStatus { get; set; } = string.Empty; // or "Status"
        public string? Comment { get; set; }              // or "Comment"
        public DateTime? Timestamp { get; set; }        // used in view (nullable)
        public DateTime? VerifiedDate { get; set; }= DateTime.UtcNow;     // optional explicit date
        public int AgentId { get; internal set; }
        public string? Status { get; set; }
        public string? Name { get; set; }   // if used in your view
        public string? TagId { get; set; }
    }
}

