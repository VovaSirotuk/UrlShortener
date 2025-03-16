using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTestProject.Services.UrlSer
{
    public interface IUrlShortService
    {

        Task<ShortenedUrl> ShortenUrlAsync(AddUrlVM model);
        Task<string?> GetOriginalUrlAsync(string shortCode);
        Task<IEnumerable<ShortenedUrl>> GetAll();
        Task<ShortenedUrl> GetByIdAsync(int id);
        Task DeleteByIdAsync(int id);
        Task DeleteAllAsync();
        Task<ShortenedUrl> GetByOriginCode(string originCode);
        
    }
}
