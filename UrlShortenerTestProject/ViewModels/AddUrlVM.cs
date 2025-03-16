using System.ComponentModel.DataAnnotations;

namespace UrlShortenerTestProject.ViewModels
{
    public class AddUrlVM
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; }

        public string CreatedBy { get; set; }
    }
}
