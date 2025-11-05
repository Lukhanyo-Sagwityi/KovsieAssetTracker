namespace KovsieAssetTracker.Controllers.API
{
    public class MobileAssetAPI

    {
        public int? id { get; set; }
        public string? tag { get; set; }
        public string? name { get; set; }
        public string? qrCode { get; set; }
        public string? photoUri { get; set; }
        public string? condition { get; set; }
        public string? location { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string? notes { get; set; }
        public string? status { get; set; }
        public string? department { get; set; }
        public string? submissionDate { get; set; }
        public string? submittedBy { get; set; }
    }
}