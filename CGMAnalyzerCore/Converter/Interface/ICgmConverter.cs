namespace CGMAnalyzerCore.Converter.Interface
{
    public interface ICgmConverter
    {
        Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string filename);
    }
}
