namespace CGMAnalyzerCore.Convert
{
    public interface ICgmConverter
    {
        Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string label);
    }
}
