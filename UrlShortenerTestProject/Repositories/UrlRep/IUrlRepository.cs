using UrlShortenerTestProject.Models;

namespace UrlShortenerTestProject.Repositories.UrlRep
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
        Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl);
        Task AddAsync(ShortenedUrl shortUrl);
        Task<IEnumerable<ShortenedUrl>> GetAll();
        Task<ShortenedUrl> GetByIdAsync(int id);
        Task DeleteByIdAsync(int id);
        Task DeleteAllAsync();
        Task SaveChangesAsync();

    }
}
