using Microsoft.EntityFrameworkCore;
using KovsieAssetTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace KovsieAssetTracker.Data
{
    public class VerificationRepository : Repository<Verification>, IVerificationRepository
    {
        private readonly AppDbContext _context;
        public VerificationRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public new async Task<Verification?> GetByIdAsync(int id)
        {
            return await _context.Verifications
                .Include(v => v.Asset)   // optional if you want asset data
                .FirstOrDefaultAsync(v => v.VerificationId == id);
        }
         public async Task<Verification?> GetByAssetIdSingleAsync(int assetId)
        {
            return await _context.Verifications
                .FirstOrDefaultAsync(v => v.AssetId == assetId);
        }

        public new async Task AddAsync(Verification verification)
        {
            _context.Verifications.Add(verification);
            await _context.SaveChangesAsync();  // ✅ must save here too
        }
        public async Task UpdateAsync(Verification verification)
        {
            if (verification == null)
                throw new ArgumentNullException(nameof(verification));

            if (verification.VerificationId <= 0)
                throw new InvalidOperationException("Invalid VerificationId provided.");

            // Find the tracked/existing entity
            var existing = await _context.Verifications.FindAsync(verification.VerificationId);
            if (existing == null)
                throw new InvalidOperationException($"Verification not found (ID={verification.VerificationId}).");

            // Copy only the values you want to update
            // This will update scalar properties. For navigation properties handle separately.
            _context.Entry(existing).CurrentValues.SetValues(verification);

            // If you need finer control, set specific fields individually:
            // existing.Status = verification.Status;
            // existing.Comment = verification.Comment;
            // existing.VerifiedDate = verification.VerifiedDate;
            // existing.AgentName = verification.AgentName;
            // etc.

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Verification>> GetByAssetIdAsync(int assetId)
        {
            return await _context.Verifications
                .Include(v => v.AgentName)
                .Include(v => v.Asset)
                .Where(v => v.AssetId == assetId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Verification>> GetByAgentIdAsync(int agentId)
        {
            return await _context.Verifications
                .Include(v => v.Asset)
                .Include(v => v.AgentName)
                .Where(v => v.AgentId == agentId)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task VerifyAssetAsync(int assetId, int userId, string agentName, string? notes)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null) throw new InvalidOperationException("Asset not found");

            // create verification record
            var verification = new Verification
            {
                AssetId = assetId,
                AgentName = agentName,
                VerificationStatus = "Verified",
                Comment = notes,
                Timestamp = DateTime.UtcNow,
                VerifiedDate = DateTime.UtcNow
            };

            _context.Verifications.Add(verification);

            // update asset
            asset.Verified = true;
            asset.UpdatedAt = DateTime.UtcNow;

            _context.Assets.Update(asset);

            await _context.SaveChangesAsync();
        }
        public new async Task<List<Verification>> GetAllAsync()
        {
            return await _context.Verifications
                .OrderByDescending(v => v.VerifiedDate)
                .ToListAsync();
        }

    }
}
