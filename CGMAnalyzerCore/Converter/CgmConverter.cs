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
        public async Task<byte[]> ConvertToBmpBytesAsync(Stream stream, string filename)
        {
            var parser = new CgmParser();
            parser.Load(stream, filename);

            var renderer = new CgmRenderer(parser.Commands);
            using var bmp = renderer.Render();

            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        //public async Task<byte[]> ConvertToBmpBytesAsync(Stream cgmStream, string label = "CGM")
        //{
        //    // Simule le rendu à partir du CGM (à remplacer par ton vrai parseur)
        //    using var bmp = new Bitmap(200, 100);
        //    using var graphics = Graphics.FromImage(bmp);
        //    graphics.Clear(Color.LightGray);
        //    graphics.DrawString(label, new Font("Arial", 14), Brushes.Black, new PointF(10, 40));

        //    using var ms = new MemoryStream();
        //    bmp.Save(ms, ImageFormat.Bmp);
        //    return ms.ToArray();
        //}

        //public async Task<string> ConvertToBmpAsync(IFormFile file)
        //{
        //// simulate treatment CGM → BMP
        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //var bmpFileName = fileName + ".bmp";
        //var imagePath = Path.Combine(_env.WebRootPath, "images", bmpFileName);

        //// Testing : To create a fake BMP img 
        //using var bmp = new System.Drawing.Bitmap(200, 100);
        //using var graphics = System.Drawing.Graphics.FromImage(bmp);
        //graphics.Clear(System.Drawing.Color.LightGray);
        //graphics.DrawString(fileName, new System.Drawing.Font("Arial", 14), System.Drawing.Brushes.Black, new PointF(10, 40));

        //bmp.Save(imagePath, System.Drawing.Imaging.ImageFormat.Bmp);

        //// return relative path to front (WPF)
        //return "/images/" + bmpFileName;
        //}
    }
}
