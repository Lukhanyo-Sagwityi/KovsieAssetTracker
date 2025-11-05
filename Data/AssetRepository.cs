using KovsieAssetTracker.Data.QueryOptions;
using KovsieAssetTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace KovsieAssetTracker.Data.Repositories
{
    public class AssetRepository : Repository<Asset>, IAssetRepository
    {
        private readonly AppDbContext _context;
        public AssetRepository(AppDbContext ctx) : base(ctx) { _context = ctx; }


        public async Task<Asset?> GetByIdAsync(int id)
        {
            return await _context.Assets.FindAsync(id);
        }

        public async Task UpdateAsync(Asset asset)
        {
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task<Asset?> GetByTagAsync(string tagId)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.TagId == tagId);
        }

        public async Task<PagedResult<Asset>> GetPagedAsync(QueryOptions<Asset> options)
        {
            options ??= new QueryOptions<Asset>();
            var query = _dbSet.AsNoTracking().AsQueryable();

            // apply Where filter if present
            if (options.HasWhere)
                query = query.Where(options.Where!);

            // compute totals
            var totalItems = await query.CountAsync();

            // calculate total pages, then cap to MaxPages
            var totalPages = (int)System.Math.Ceiling(totalItems / (double)options.PageSize);
            var allowedPages = System.Math.Min(totalPages == 0 ? 1 : totalPages, options.MaxPages);

            // ensure page number is within range
            var page = options.PageNumber;
            if (page < 1) page = 1;
            if (page > allowedPages) page = allowedPages;

            // Sorting: if SortBy is provided, try to order by that property name (basic reflection)
            if (options.HasSorting)
            {
                var prop = typeof(Asset).GetProperty(options.SortBy!, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    query = options.SortDescending
                        ? query.OrderByDescending(e => EF.Property<object>(e, prop.Name))
                        : query.OrderBy(e => EF.Property<object>(e, prop.Name));
                }
                else
                {
                    query = query.OrderBy(a => a.TagId); // fallback
                }
            }
            else
            {
                query = query.OrderBy(a => a.TagId);
            }

            var items = await query.Skip((page - 1) * options.PageSize)
                                   .Take(options.PageSize)
                                   .ToListAsync();

            return new PagedResult<Asset>
            {
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = options.PageSize,
                TotalPages = allowedPages,
                Items = items
            };
        }

        public Task<IEnumerable<Asset>> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Asset>> SearchAsync(string? tagFilter, string? locationFilter, string? conditionFilter)
        {
            var query = _context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tagFilter))
                query = query.Where(a => a.TagId.Contains(tagFilter));

            if (!string.IsNullOrWhiteSpace(locationFilter))
                query = query.Where(a => a.Location.Contains(locationFilter));

            if (!string.IsNullOrWhiteSpace(conditionFilter))
                query = query.Where(a => a.Condition.Contains(conditionFilter));

            return await query.ToListAsync();
        }

        public async Task<List<Asset>> GetPendingAssetsAsync()
        {
            return await _context.Assets
                .Where(a => a.VerificationStatus == "Pending")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }


        public async Task<List<Asset>> GetVerifiedAssetsAsync()
        {
            return await _context.Assets
                .Where(a => a.VerificationStatus != "Pending")
                .ToListAsync();
        }

        public async Task UpdateVerificationStatusAsync(int verificationId, string status, int agentId, string? agentName, string? comment)
        {
            var verification = await _context.Verifications
                                             .FirstOrDefaultAsync(v => v.VerificationId == verificationId);

            if (verification == null)
            {
                throw new InvalidOperationException($"Verification with ID {verificationId} not found.");
            }

            verification.Status = status;
            verification.AgentId = agentId;

            // Only set AgentName if your entity has this property
            if (!string.IsNullOrWhiteSpace(agentName))
                verification.AgentName = agentName;

            verification.Comment = comment;
            verification.Timestamp = DateTime.UtcNow; // optional: update timestamp

            await _context.SaveChangesAsync();
        }
        public async Task<Asset?> GetAsync(int id)
        {
            return await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == id);
        }
        public void Add(Asset asset)
        {
            _context.Assets.Add(asset);
        }

    }
}
