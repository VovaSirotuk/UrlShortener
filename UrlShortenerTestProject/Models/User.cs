using Microsoft.AspNetCore.Identity;

namespace UrlShortenerTestProject.Models
{
	public class User : IdentityUser
	{
		public string FullName { get; set; }
	}
}
