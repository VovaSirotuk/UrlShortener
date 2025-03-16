using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTestProject.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<User> signInManager;
		private readonly UserManager<User> userManager;

		public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM model) 
		{
			if (ModelState.IsValid) 
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Email чи пароль не вірні.");
					return View(model);
				}
			}
			return View(model);
		}
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM model) 
		{
			if (ModelState.IsValid)
			{
				User user = new User
				{
					FullName = model.Name,
					Email = model.Email,
					UserName = model.Email,
				};

				var result = await userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					return RedirectToAction("Login", "Account");
				}
				else
				{
					foreach (var item in result.Errors) 
					{
						ModelState.AddModelError("", item.Description);
					}

					return View(model);
				}
			}
			return View(model);

		}
		public async Task<IActionResult> Logout() {
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
