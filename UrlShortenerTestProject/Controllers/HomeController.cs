using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.UrlRep;
using UrlShortenerTestProject.Services.UrlSer;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTestProject.Controllers
{
    public class HomeController : Controller
	{
		private readonly IUrlShortService urlShortService;
        private readonly UserManager<User> userManager;

        public HomeController(IUrlShortService urlShortService, UserManager<User> userManager)
		{
			this.urlShortService = urlShortService;
			this.userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}
        public async Task<IActionResult> Table()
        {
			var allUrl = await urlShortService.GetAll();
            return View(allUrl);
        }
        [HttpPost]
		[Authorize]
		public async Task<IActionResult> index(AddUrlVM model)
		{
			var shortenedUrl = await urlShortService.ShortenUrlAsync(model);
			return View(shortenedUrl);
		}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShortenUrlFromTable([FromBody] AddUrlVM model)
        {
            if (string.IsNullOrWhiteSpace(model.OriginalUrl))
            {
                return BadRequest("URL не може бути порожнім!");
            }
            if (await urlShortService.GetByOriginCode(model.OriginalUrl) != null) 
            {
                return BadRequest( new { error = "Таке посилання вже є в таблиці!" });
            }
			var userName = User.Identity.Name;
			model.CreatedBy = userName;
            var shortenedUrl = await urlShortService.ShortenUrlAsync(model);
            if (shortenedUrl == null)
            {
                return StatusCode(500, "Error creating shortened URL");
            }

            return Ok(new
            {
				id = shortenedUrl.Id,
                originalUrl = shortenedUrl.OriginalUrl,
                shortUrl = $"{Request.Scheme}://{Request.Host}/{shortenedUrl.ShortCode}",
                createdBy = shortenedUrl.CreatedBy,
                createdAt = shortenedUrl.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                clickCount = shortenedUrl.ClickCount
            });;
        }
        [HttpGet("{shortUrl}")]
		public async Task<IActionResult> RedirectToLongUrl(string shortUrl)
		{
			var longUrl = await urlShortService.GetOriginalUrlAsync(shortUrl);

			if (longUrl == null)
			{
				return NotFound("Скорочене посилання не знайдено.");
			}

			return Redirect(longUrl);
		}
		public async Task<IActionResult> Details(int id) {

			var url = await urlShortService.GetByIdAsync(id);
			return View(url);
		}
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
			try 
			{
                await urlShortService.DeleteByIdAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
			{
                return Json(new { success = false, errorMessage = ex.Message });
            }
            			
        }
        [HttpPost]
        public async Task<JsonResult> DeleteAll()
        {
            try
            {
                await urlShortService.DeleteAllAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
	}
}
