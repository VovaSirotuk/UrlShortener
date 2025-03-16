using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.UrlRep;

namespace UrlShortenerTests
{
    public class UrlRepositoryTests
    {
        private async Task<AppDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Унікальна БД для кожного тесту
                .Options;

            var databaseContext = new AppDbContext(options);
            await databaseContext.Database.EnsureCreatedAsync(); // Створюємо БД

            return databaseContext;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Url()
        {
            await using var context = await GetDatabaseContext();
            var repository = new UrlRepository(context);
            var shortUrl = new ShortenedUrl
            {
                OriginalUrl = "https://example.com",
                ShortCode = "abc123",
                CreatedBy = "user@user.com"
            };

            await repository.AddAsync(shortUrl);
            await context.SaveChangesAsync(); // Оновлюємо БД

            var result = await repository.GetByShortCodeAsync("abc123");

            Assert.NotNull(result);
            Assert.Equal("https://example.com", result.OriginalUrl);
        }

        [Fact]
        public async Task GetByShortCodeAsync_Should_Return_Url()
        {
            await using var context = await GetDatabaseContext();
            var repository = new UrlRepository(context);
            var shortUrl = new ShortenedUrl { OriginalUrl = "https://test.com", ShortCode = "test1", CreatedBy = "user@user.com" };

            await repository.AddAsync(shortUrl);
            await context.SaveChangesAsync(); // Оновлюємо БД

            var result = await repository.GetByShortCodeAsync("test1");

            Assert.NotNull(result);
            Assert.Equal("https://test.com", result.OriginalUrl);
        }

        [Fact]
        public async Task GetByOriginalUrlAsync_Should_Return_Url()
        {
            await using var context = await GetDatabaseContext();
            var repository = new UrlRepository(context);
            var shortUrl = new ShortenedUrl { OriginalUrl = "https://original.com", ShortCode = "orig1", CreatedBy = "user@user.com" };

            await repository.AddAsync(shortUrl);
            await context.SaveChangesAsync(); // Оновлюємо БД

            var result = await repository.GetByOriginalUrlAsync("https://original.com");

            Assert.NotNull(result);
            Assert.Equal("orig1", result.ShortCode);
        }

        [Fact]
        public async Task GetAll_Should_Return_All_Urls()
        {
            await using var context = await GetDatabaseContext();
            var repository = new UrlRepository(context);

            await repository.AddAsync(new ShortenedUrl { OriginalUrl = "https://one.com", ShortCode = "one" , CreatedBy = "user@user.com" });
            await repository.AddAsync(new ShortenedUrl { OriginalUrl = "https://two.com", ShortCode = "two", CreatedBy = "user@user.com" });
            await context.SaveChangesAsync(); // Оновлюємо БД

            var result = await repository.GetAll(); // Конвертуємо в список

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Remove_Url()
        {
            await using var context = await GetDatabaseContext();
            var repository = new UrlRepository(context);
            var shortUrl = new ShortenedUrl { OriginalUrl = "https://delete.com", ShortCode = "del1" , CreatedBy = "user@user.com" };

            await repository.AddAsync(shortUrl);
            await context.SaveChangesAsync(); // Оновлюємо БД

            await repository.DeleteByIdAsync(shortUrl.Id);
            await context.SaveChangesAsync(); // Оновлюємо БД після видалення

            var result = await repository.GetByIdAsync(shortUrl.Id);

            Assert.Null(result);
        }
    }
}
