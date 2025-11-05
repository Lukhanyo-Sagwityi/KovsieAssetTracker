using KovsieAssetTracker.Controllers.Dto;
using KovsieAssetTracker.Data;
using KovsieAssetTracker.Data.Repositories;
using KovsieAssetTracker.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KovsieAssetTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // Simple login - returns basic user info (no token/session yet)
        // POST api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null) return Unauthorized(new { Message = "Invalid credentials" });

            if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(new { Message = "Invalid credentials" });

            // In production you should return a JWT or set a session cookie.
            return Ok(new { user.UserId, user.Name, user.Email, user.Role });
        }

        // GET api/users  (list users) - admin-only ideally (no auth here)
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users);
        }
    }
}
