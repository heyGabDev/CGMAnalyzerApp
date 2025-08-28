namespace CGMAnalyzerCore.Converter.Interface
{
    public interface ICgmConverter
    {
        /// <summary>
        /// Convertit un stream CGM en bytes BMP
        /// </summary>
        Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string fileName);

        /// <summary>
        /// Convertit avec des options de rendu personnalisées
        /// </summary>
        Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string fileName, RenderOptions options);
    }

    public class RenderOptions
    {
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public bool HighQuality { get; set; } = true;
        public System.Drawing.Color BackgroundColor { get; set; } = System.Drawing.Color.White;
        public List<string>? VisibleLayers { get; set; } // null = tous les layers
    }
}

