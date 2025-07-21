namespace CGMAnalyzer.API.Services.Interfaces
{
    public interface ICgmImageService
    {
        Task<string> GenerateAndSaveBmpAsync(IFormFile file);
    }
}
