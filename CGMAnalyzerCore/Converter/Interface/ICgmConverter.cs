using CGMAnalyzerCore.Render; // Ajouter ce using

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

    // SUPPRIMER cette classe RenderOptions d'ici
}