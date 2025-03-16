using UrlShortenerTestProject.Models;

namespace UrlShortenerTestProject.Repositories.AboutRep
{
    public interface IAboutRepository
    {
        Task<AboutInfo?> GetAboutInfoAsync();
        Task UpdateAboutInfoAsync(string newDescription);
    }
}
