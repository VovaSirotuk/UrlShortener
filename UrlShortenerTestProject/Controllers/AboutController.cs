using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerTestProject.Services.AboutService;

namespace UrlShortenerTestProject.Controllers
{
    [Route("About")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var description = await _aboutService.GetDescriptionAsync();
            return View("Index", description);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string newDescription)
        {
            if (!string.IsNullOrWhiteSpace(newDescription))
            {
                await _aboutService.UpdateDescriptionAsync(newDescription);
            }

            return RedirectToAction("Index");
        }
    }
}
