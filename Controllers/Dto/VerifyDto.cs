namespace KovsieAssetTracker.Controllers.Dto
{
    public class VerifyDto
    {
        public int AssetId { get; set; }
        public int? AgentId { get; set; }
        public string? Status { get; set; } = "Verified"; // or "Rejected"
        public string? Notes { get; set; }
    }
}
