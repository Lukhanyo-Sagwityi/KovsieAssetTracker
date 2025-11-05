using Microsoft.AspNetCore.Mvc;
using KovsieAssetTracker.Data;
using KovsieAssetTracker.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using KovsieAssetTracker.Controllers.API;

namespace KovsieAssetTracker.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsApiController : ControllerBase
    {
        private readonly IAssetRepository _assetRepo;
        private readonly IWebHostEnvironment _env;

        public AssetsApiController(IAssetRepository assetRepo, IWebHostEnvironment env)
        {
            _assetRepo = assetRepo;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? since = null)
        {
            DateTimeOffset? filter = null;
            if (!string.IsNullOrWhiteSpace(since) && DateTimeOffset.TryParse(since, out var parsed))
                filter = parsed;

            if (filter.HasValue)
            {
                var list = await _assetRepo.SearchAsync(null, null, null);
                var filtered = list.Where(a => a.UpdatedAt.HasValue && a.UpdatedAt.Value > filter.Value.UtcDateTime);
                return Ok(filtered);
            }
            else
            {
                var all = await _assetRepo.SearchAsync(null, null, null);
                return Ok(all);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MobileAssetAPI dto)
        {
            if (dto == null) return BadRequest("No data received");
            if (string.IsNullOrWhiteSpace(dto.tag) && string.IsNullOrWhiteSpace(dto.qrCode))
            {

                return BadRequest("Tag or QR code required");
            }


            KovsieAssetTracker.Models.Asset? asset = null;
            if (!string.IsNullOrWhiteSpace(dto.tag))
            {
                asset = await _assetRepo.GetByTagAsync(dto.tag);
            }

            var isNew = asset == null;
            if (isNew)
            {
                asset = new KovsieAssetTracker.Models.Asset
                {
                    TagId = dto.tag,
                    Name = dto.name,
                    Description = dto.notes,
                    Location = dto.location,
                    Condition = dto.condition,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _assetRepo.Add(asset);
            }
            else
            {
                // update fields
                asset.Name = dto.name ?? asset.Name;
                asset.Description = dto.notes ?? asset.Description;
                asset.Location = dto.location ?? asset.Location;
                asset.Condition = dto.condition ?? asset.Condition;
                asset.UpdatedAt = DateTime.UtcNow;
                _assetRepo.Update(asset);
            }


            if (!string.IsNullOrWhiteSpace(dto.photoUri))
            {
                var val = dto.photoUri.Trim();
                if (val.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var m = Regex.Match(val, @"data:(image\/[a-zA-Z]+);base64,(.+)");
                        if (m.Success)
                        {
                            var mime = m.Groups[1].Value;
                            var base64 = m.Groups[2].Value;
                            var bytes = Convert.FromBase64String(base64);
                            var ext = mime.Contains("jpeg") || mime.Contains("jpg") ? ".jpg" :
                                      mime.Contains("png") ? ".png" : ".bin";
                            var fileName = $"asset_{Guid.NewGuid()}{ext}";
                            var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                            Directory.CreateDirectory(uploadsDir);
                            var savePath = Path.Combine(uploadsDir, fileName);
                            await System.IO.File.WriteAllBytesAsync(savePath, bytes);
                            asset.PhotoPath = $"/uploads/{fileName}";
                        }
                        else
                        {

                        }
                    }
                    catch
                    {

                    }
                }
                else if (Uri.IsWellFormedUriString(val, UriKind.Absolute))
                {
                    asset.PhotoPath = val;
                }
                else
                {

                }
            }

            await _assetRepo.SaveAsync();


            return CreatedAtAction(nameof(GetById), new { id = asset.AssetId }, new
            {
                id = asset.AssetId,
                tag = asset.TagId,
                name = asset.Name,
                photoPath = asset.PhotoPath,
                updatedAt = asset.UpdatedAt
            });
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asset = await _assetRepo.GetAsync(id);
            if (asset == null) return NotFound();
            return Ok(asset);
        }

        [HttpPost("{id:int}/verify")]
        public async Task<IActionResult> Verify(int id, [FromBody] VerifyAgentDto dto)
        {
            var asset = await _assetRepo.GetAsync(id);
            if (asset == null) return NotFound();
            asset.IsVerified = true;
            asset.VerifiedById = dto.AgentId;
            asset.VerifiedAt = DateTime.UtcNow;
            _assetRepo.Update(asset);
            await _assetRepo.SaveAsync();
            return Ok(new { success = true });
        }

        public class VerifyAgentDto
        {
            public int? AgentId { get; set; }
            public string? Notes { get; set; }
        }
    }
}