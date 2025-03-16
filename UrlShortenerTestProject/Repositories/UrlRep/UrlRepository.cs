using Microsoft.EntityFrameworkCore;
using System;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;

namespace UrlShortenerTestProject.Repositories.UrlRep
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _context;

        public UrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
        {
            var url = await _context.ShortenedUrl.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
            return url;
        }

        public async Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl)
        {
            return await _context.ShortenedUrl.FirstOrDefaultAsync(x => x.OriginalUrl == originalUrl);
        }

        public async Task AddAsync(ShortenedUrl shortUrl)
        {
            _context.ShortenedUrl.Add(shortUrl);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ShortenedUrl>> GetAll()
        {
            var allUrl = await _context.ShortenedUrl.ToListAsync();
            return allUrl;
        }
        public async Task<ShortenedUrl> GetByIdAsync(int id) 
        {
            return await _context.ShortenedUrl.FindAsync(id);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var url = await _context.ShortenedUrl.FindAsync(id);
            if (url != null)
                _context.ShortenedUrl.Remove(url);
                await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE [ShortenedUrl]");
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
