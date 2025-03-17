using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.AboutRep;

namespace UrlShortenerTests
{
    public class AboutRepositoryTests
    {
        private async Task<AppDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Унікальна БД для кожного тесту
                .Options;

            var databaseContext = new AppDbContext(options);
            await databaseContext.Database.EnsureCreatedAsync();
            return databaseContext;
        }

        [Fact]
        public async Task GetAboutInfoAsync_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new AboutRepository(dbContext);

            // Act
            var result = await repository.GetAboutInfoAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAboutInfoAsync_ReturnsData_WhenDataExists()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var aboutInfo = new AboutInfo { Description = "Test Description" };
            dbContext.AboutInfos.Add(aboutInfo);
            await dbContext.SaveChangesAsync();

            var repository = new AboutRepository(dbContext);

            // Act
            var result = await repository.GetAboutInfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Description", result.Description);
        }

        [Fact]
        public async Task UpdateAboutInfoAsync_AddsNewRecord_WhenNoDataExists()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new AboutRepository(dbContext);

            // Act
            await repository.UpdateAboutInfoAsync("New Description");
            var result = await repository.GetAboutInfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Description", result.Description);
        }

        [Fact]
        public async Task UpdateAboutInfoAsync_UpdatesExistingRecord_WhenDataExists()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var aboutInfo = new AboutInfo { Description = "Old Description" };
            dbContext.AboutInfos.Add(aboutInfo);
            await dbContext.SaveChangesAsync();

            var repository = new AboutRepository(dbContext);

            // Act
            await repository.UpdateAboutInfoAsync("Updated Description");
            var result = await repository.GetAboutInfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
        }
    }
}
