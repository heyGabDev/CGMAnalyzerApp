namespace CGMAnalyzer.API.Services
{
    public interface ICGMLayerDetector
    {
        List<int> DetectLayers(string filePath);
    }
}