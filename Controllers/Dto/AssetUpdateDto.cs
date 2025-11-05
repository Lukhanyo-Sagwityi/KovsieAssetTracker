namespace KovsieAssetTracker.Controllers.Dto
{
    public class AssetUpdateDto
    {
        public int AssetId { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public string? Location { get; set; }
    }
}
