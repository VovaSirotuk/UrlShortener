using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.UrlRep;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTestProject.Services.UrlSer
{
    public class UrlShortService : IUrlShortService
    {
        public const int NumberOfChatsInShortLink = 6;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new();

        private readonly IUrlRepository urlRepository;

        public UrlShortService(IUrlRepository urlRepository)
        {
            this.urlRepository = urlRepository;
        }

        private string GenerateBase62Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Використовуємо тільки позитивну частину хешу
                BigInteger number = new BigInteger(hashBytes.Reverse().ToArray()); // Робимо порядок байтів правильним

                if (number < 0) number = -number; // Виключаємо від'ємні значення

                StringBuilder result = new StringBuilder();
                do
                {
                    result.Insert(0, Alphabet[(int)(number % Alphabet.Length)]);
                    number /= Alphabet.Length;
                } while (number > 0);

                return result.ToString().Substring(0, Math.Min(result.Length, 6)); // Забезпечуємо мінімальну довжину
            }
        }
        public async Task<string> GenerateUniqueCodeAsync(string url)
        {
            while (true)
            {
                var shortCode = GenerateBase62Hash(url);

                if (await urlRepository.GetByShortCodeAsync(shortCode) == null)
                {
                    return shortCode;
                }
            }
        }
        public async Task<ShortenedUrl> ShortenUrlAsync(AddUrlVM model)
        {
            var existing = await urlRepository.GetByOriginalUrlAsync(model.OriginalUrl);
            if (existing != null)
                return existing;

            var shortCode = await GenerateUniqueCodeAsync(model.OriginalUrl);
            var url = new ShortenedUrl
            {
                OriginalUrl = model.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.CreatedBy
            };

            await urlRepository.AddAsync(url);
            return url;
        }
        public async Task<ShortenedUrl> GetByOriginCode(string originCode) 
        {
            var url = await urlRepository.GetByOriginalUrlAsync(originCode);
            return url;
        }

        public async Task<string?> GetOriginalUrlAsync(string shortCode)
        {
            var url = await urlRepository.GetByShortCodeAsync(shortCode);
            if (url == null)
            {
                return null;
            }

            url.ClickCount++;
            await urlRepository.SaveChangesAsync();
            return url.OriginalUrl;
        }

        public async Task<IEnumerable<ShortenedUrl>> GetAll() 
        {
            var allUrl = await urlRepository.GetAll();
            return allUrl;
        }

        public async Task<ShortenedUrl> GetByIdAsync(int id) 
        {
            var url = await urlRepository.GetByIdAsync(id);
            return url;
        }
        public async Task DeleteByIdAsync(int id)
        {
            await urlRepository.DeleteByIdAsync(id);
        }
        public async Task DeleteAllAsync()
        {
            await urlRepository.DeleteAllAsync();
        }
    }
}
