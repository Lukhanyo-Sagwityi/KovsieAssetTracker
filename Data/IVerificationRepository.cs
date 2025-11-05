using System.Collections.Generic;
using System.Threading.Tasks;
using KovsieAssetTracker.Models;

namespace KovsieAssetTracker.Data
{
    public interface IVerificationRepository : IRepository<Verification>
    {
        // Get all verifications for a specific asset
        Task<IEnumerable<Verification>> GetByAssetIdAsync(int assetId);
        Task UpdateAsync(Verification verification);

        // Get all verifications performed by a specific agent
        Task<IEnumerable<Verification>> GetByAgentIdAsync(int agentId);

        // Verify (approve) an asset and record a verification entry
        Task VerifyAssetAsync(int assetId, int userId, string agentName, string? notes);
        Task<List<Verification>> GetAllAsync();

    }
}
