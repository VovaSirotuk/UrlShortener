using System.ComponentModel.DataAnnotations;

namespace UrlShortenerTestProject.Models
{
    public class AboutInfo
    {
        public int Id { get; set; } // Первинний ключ

        [Required]
        public string Description { get; set; } = string.Empty;
        
    }
}
