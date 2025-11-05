using KovsieAssetTracker.Data.QueryOptions;
using KovsieAssetTracker.Models;
using System.Threading.Tasks;

namespace KovsieAssetTracker.Data
{
    public interface IAssetRepository : IRepository<Asset>
    {
        Task<PagedResult<Asset>> GetPagedAsync(QueryOptions<Asset> options);
        Task<Asset?> GetByTagAsync(string tagId);
        Task<IEnumerable<Asset>> SearchAsync(string? tagFilter, string? locationFilter, string? conditionFilter);
        Task<List<Asset>> GetPendingAssetsAsync();
        Task<List<Asset>> GetVerifiedAssetsAsync();
        Task UpdateAsync(Asset asset);
        Task UpdateVerificationStatusAsync(int verificationId, string status, int agentId, string? agentName, string? comment);
        new Task<Asset?> GetByIdAsync(int id);
        Task<Asset?> GetAsync(int id);
        void Add(Asset asset);
    }
}
