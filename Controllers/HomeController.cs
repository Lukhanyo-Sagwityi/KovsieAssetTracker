using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KovsieAssetTracker.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        // Optional: a dashboard redirect for logged-in users
        [Authorize]
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index", "Assets"); // or your dashboard view
        }
    }
}
