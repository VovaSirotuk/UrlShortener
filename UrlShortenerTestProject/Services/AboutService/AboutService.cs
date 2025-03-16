using Microsoft.EntityFrameworkCore;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.AboutRep;

namespace UrlShortenerTestProject.Services.AboutService
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;

        public AboutService(IAboutRepository aboutRepository)
        {
            _aboutRepository = aboutRepository;
        }

        public async Task<string> GetDescriptionAsync()
        {
            var about = await _aboutRepository.GetAboutInfoAsync();
            return about?.Description ?? "No description available.";
        }

        public async Task UpdateDescriptionAsync(string newDescription)
        {
            await _aboutRepository.UpdateAboutInfoAsync(newDescription);
        }

    }
}
