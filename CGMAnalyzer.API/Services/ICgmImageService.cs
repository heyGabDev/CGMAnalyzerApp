namespace CGMAnalyzer.API.Services
{
    public interface ICgmImageService
    {
        Task<string> GenerateAndSaveBmpAsync(IFormFile file);
    }
}
