using CGMAnalyzerCore.Modeles;

// CGMAnalyzer.API/Services/Interfaces/ICgmImageService.cs
namespace CGMAnalyzer.API.Services.Interfaces
{
    public interface ICgmImageService
    {
        Task<string> GenerateAndSaveBmpAsync(IFormFile file);

        // Nouvelles méthodes (optionnelles pour la compatibilité)
        void ClearOldCacheEntries(TimeSpan maxAge);
        int GetCacheSize();
        void ClearAllCache();
        bool IsFileInCache(IFormFile file);
    }
}