using Microsoft.AspNetCore.Http;

namespace KovsieAssetTracker.Controllers.Dto
{
    public class AssetSubmissionDto
    {
        public string? TagId { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public string? Location { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
