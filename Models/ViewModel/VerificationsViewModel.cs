namespace KovsieAssetTracker.Models.ViewModel
{
    public class VerificationsViewModel
    {
        public List<KovsieAssetTracker.Models.Asset> PendingAssets { get; set; } = new();
        public List<KovsieAssetTracker.Models.Verification> VerificationLog { get; set; } = new();
        public List<Verification> PendingVerifications { get; set; } = new();

    }
}
