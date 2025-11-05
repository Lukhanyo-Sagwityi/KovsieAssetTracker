using KovsieAssetTracker.Controllers.Dto;
using KovsieAssetTracker.Data;
using KovsieAssetTracker.Data.QueryOptions;
using KovsieAssetTracker.Data.Repositories;
using KovsieAssetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

[Authorize] // require login for dashboard
public class AssetsController : Controller
{
    private readonly IAssetRepository _assetRepo;

    public AssetsController(IAssetRepository assetRepo)
    {
        _assetRepo = assetRepo;
    }

    // GET /Assets
    [Authorize] // optional: allow anonymous to view list if you want public listing
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5, string? tagFilter = null, string? roomFilter = null, string? conditionFilter = null)
    {
        var options = new QueryOptions<Asset>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = "TagId",
            SortDescending = false,
            MaxPages = 3
        };

        Expression<Func<Asset, bool>>? where = null;
        if (!string.IsNullOrWhiteSpace(tagFilter)) where = a => a.TagId.Contains(tagFilter);
        if (!string.IsNullOrWhiteSpace(roomFilter))
        {
            var prev = where;
            Expression<Func<Asset, bool>> add = a => a.Location.Contains(roomFilter);
            where = prev == null ? add : Combine(prev, add);
        }
        if (!string.IsNullOrWhiteSpace(conditionFilter))
        {
            var prev = where;
            Expression<Func<Asset, bool>> add = a => a.Condition == conditionFilter;
            where = prev == null ? add : Combine(prev, add);
        }
        if (where != null) options.Where = where;

        var paged = await _assetRepo.GetPagedAsync(options);
        return View(paged); // Views/Assets/Index.cshtml expects PagedResult<Asset>
    }

    // GET /Assets/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var asset = await _assetRepo.GetByIdAsync(id);
        if (asset == null) return NotFound();
        return View(asset);
    }

    // GET /Assets/Create
    [Authorize]
    public IActionResult Create() => View();

    // POST /Assets/Create
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Asset model)
    {
        if (!ModelState.IsValid) return View(model);
        await _assetRepo.AddAsync(model);
        await _assetRepo.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Assets/Edit/5
    // using KovsieAssetTracker.Controllers.Dto;
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var asset = await _assetRepo.GetByIdAsync(id);
        if (asset == null) return NotFound();

        var dto = new AssetUpdateDto
        {
            AssetId = asset.AssetId,
            Description = asset.Description,
            Location = asset.Location,
            Condition = asset.Condition
            // add any other fields you need to edit
        };

        return View(dto); // view expects AssetUpdateDto
    }


    // POST /Assets/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(AssetUpdateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var asset = await _assetRepo.GetByIdAsync(dto.AssetId);
        if (asset == null) return NotFound();

        asset.Description = dto.Description;
        asset.Location = dto.Location;
        asset.Condition = dto.Condition;
        asset.UpdatedAt = DateTime.UtcNow;

        _assetRepo.Update(asset);
        await _assetRepo.SaveAsync();

        return RedirectToAction(nameof(Details), new { id = dto.AssetId });
    }


    // POST /Assets/Delete/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var asset = await _assetRepo.GetByIdAsync(id);
        if (asset == null) return NotFound();
        _assetRepo.Delete(asset);
        await _assetRepo.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    // helper to combine expressions
    private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(Expression.Invoke(a, param), Expression.Invoke(b, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
