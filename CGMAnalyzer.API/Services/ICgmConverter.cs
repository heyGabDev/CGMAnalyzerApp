namespace CGMAnalyzer.API.Services
{
    public interface ICgmConverter
    {
        Task<string> ConvertToBmpAsync(IFormFile file);
    }
}
