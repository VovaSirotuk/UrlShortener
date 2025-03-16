namespace UrlShortenerTestProject.Services.AboutService
{
    public interface IAboutService
    {
        Task<string> GetDescriptionAsync();
        Task UpdateDescriptionAsync(string newDescription);
    }
}
