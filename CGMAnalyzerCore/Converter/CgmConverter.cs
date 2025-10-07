using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using System.Drawing;
using System.Drawing.Imaging;   
using System.IO;


namespace CGMAnalyzerCore.Convert
{
    public class CgmConverter : ICgmConverter
    {
        public async Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string fileName)
        {
            return await ConvertToBmpBytesAsync(cgmStream, fileName, new RenderOptions());
        }

        public async Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string fileName, RenderOptions options)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 1. Parser le CGM
                    var parser = new CgmParser();
                    parser.Load(cgmStream, fileName);

                    // 2. Créer le renderer avec les options
                    using var renderer = new CgmRenderer(parser.Commands, options);

                    // 3. Générer le bitmap
                    using var bitmap = renderer.Render();

                    // 4. Convertir en bytes BMP
                    using var memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, ImageFormat.Bmp);

                    return memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Erreur lors de la conversion CGM->BMP: {ex.Message}", ex);
                }
            });
        }
    }
}
