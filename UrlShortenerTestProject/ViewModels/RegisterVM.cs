using System.ComponentModel.DataAnnotations;

namespace UrlShortenerTestProject.ViewModels
{
	public class RegisterVM
	{
		[Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; }
		
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[StringLength(20, MinimumLength = 4)]
		[Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required.")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}
}
