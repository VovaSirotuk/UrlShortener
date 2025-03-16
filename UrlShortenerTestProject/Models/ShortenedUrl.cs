using System.ComponentModel.DataAnnotations;

namespace UrlShortenerTestProject.Models
{
	public class ShortenedUrl
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Url]
		public string OriginalUrl { get; set; }

		[Required]
		[StringLength(10)]
		public string ShortCode { get; set; }

		public string CreatedBy { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public int ClickCount { get; set; } = 0;
    }
}
