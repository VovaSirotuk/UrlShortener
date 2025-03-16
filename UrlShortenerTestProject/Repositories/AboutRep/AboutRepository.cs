using Microsoft.EntityFrameworkCore;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;

namespace UrlShortenerTestProject.Repositories.AboutRep
{
    public class AboutRepository : IAboutRepository
    {
        private readonly AppDbContext _context;

        public AboutRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AboutInfo?> GetAboutInfoAsync()
        {
            return await _context.AboutInfos.FirstOrDefaultAsync();
        }

        public async Task UpdateAboutInfoAsync(string newDescription)
        {
            var about = await _context.AboutInfos.FirstOrDefaultAsync();
            if (about == null)
            {
                about = new AboutInfo { Description = newDescription };
                _context.AboutInfos.Add(about);
            }
            else
            {
                about.Description = newDescription;
                _context.AboutInfos.Update(about);
            }

            await _context.SaveChangesAsync();
        }
    }
}
