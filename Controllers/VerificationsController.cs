using KovsieAssetTracker.Data;
using KovsieAssetTracker.Models;
using KovsieAssetTracker.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

public class VerificationsController : Controller
{
    private readonly IAssetRepository _assetRepo;
    private readonly IVerificationRepository _verificationRepo;
    private readonly IUserRepository _userRepo;

    public VerificationsController(
        IAssetRepository assetRepo,
        IVerificationRepository verificationRepo,
        IUserRepository userRepo)
    {
        _assetRepo = assetRepo;
        _verificationRepo = verificationRepo;
        _userRepo = userRepo;
    }

    // ✅ Combined page for pending assets + verification logs
    public async Task<IActionResult> Index()
    {
        var model = new VerificationsViewModel
        {
            PendingAssets = await _assetRepo.GetPendingAssetsAsync(),
            VerificationLog = await _verificationRepo.GetAllAsync()
        };

        return View(model);
    }

    // ✅ Accept an asset verification
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int assetId, string? comment)
    {
        var asset = await _assetRepo.GetByIdAsync(assetId);
        if (asset == null)
        {
            TempData["Error"] = "Asset not found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // ✅ Update asset record
            asset.Verified = true;
            asset.VerificationStatus = "Accepted";
            await _assetRepo.UpdateAsync(asset);

            // ✅ Create verification record
            var verification = new Verification
            {
                AssetId = asset.AssetId,
                Status = "Accepted",
                Comment = comment,
                AgentName = User?.Identity?.Name ?? "Admin",
                VerifiedDate = DateTime.UtcNow
            };

            await _verificationRepo.AddAsync(verification);

            TempData["Success"] = $"Asset '{asset.Name}' accepted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error while accepting: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decline(int assetId, string? comment)
    {
        var asset = await _assetRepo.GetByIdAsync(assetId);
        if (asset == null)
        {
            TempData["Error"] = "Asset not found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // ✅ Update asset record
            asset.Verified = false;
            asset.VerificationStatus = "Declined";
            await _assetRepo.UpdateAsync(asset);

            // ✅ Create verification record
            var verification = new Verification
            {
                AssetId = asset.AssetId,
                Status = "Declined",
                Comment = comment,
                AgentName = User?.Identity?.Name ?? "Admin",
                VerifiedDate = DateTime.UtcNow
            };

            await _verificationRepo.AddAsync(verification);

            TempData["Success"] = $"Asset '{asset.Name}' declined.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error while declining: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
