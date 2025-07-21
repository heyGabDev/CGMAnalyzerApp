namespace CGMAnalyzer.API.Services.Interfaces
{
    public interface ICGMLayerDetector
    {
        List<int> DetectLayers(string filePath);
    }
}