using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Services.UrlSer;

namespace UrlShortenerTestProject.Data
{
    public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions options) : base(options) 
		{

		}
		public DbSet<ShortenedUrl> ShortenedUrl { get; set; }
        public DbSet<AboutInfo> AboutInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ShortenedUrl>(builder =>
			{
				builder.Property(s => s.ShortCode).HasMaxLength(UrlShortService.NumberOfChatsInShortLink);
				builder.HasIndex(s => s.ShortCode).IsUnique();
			});
		}
	}
}
