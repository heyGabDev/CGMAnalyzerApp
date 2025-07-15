using CGMAnalyzerCore.Convert;
using System.Drawing;

namespace CGMAnalyzer.API.Services
{
    public class CgmImageService
    {
        private readonly ICgmConverter _converter;
        private readonly IWebHostEnvironment _env;

        public CgmImageService(ICgmConverter converter, IWebHostEnvironment env)
        {
            _converter = converter;
            _env = env;
        }

        public async Task<string> GenerateAndSaveBmpAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var bmpFileName = fileName + ".bmp";
            var outputPath = Path.Combine(_env.WebRootPath, "images");

            Directory.CreateDirectory(outputPath);

            var bmpPath = Path.Combine(outputPath, bmpFileName);
            using var stream = file.OpenReadStream();
            var bmpBytes = await _converter.ConvertToBmpBytesAsync(stream, fileName);

            await File.WriteAllBytesAsync(bmpPath, bmpBytes);

            return "/images/" + bmpFileName; // relative path
        }

        //public async Task<string> ConvertToBmpAsync(IFormFile file)
        //{
        //    // simulate treatment CGM → BMP
        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //    var bmpFileName = fileName + ".bmp";
        //    var imagePath = Path.Combine(_env.WebRootPath, "images", bmpFileName);

        //    // Testing : To create a fake BMP img 
        //    using var bmp = new System.Drawing.Bitmap(200, 100);
        //    using var graphics = System.Drawing.Graphics.FromImage(bmp);
        //    graphics.Clear(System.Drawing.Color.LightGray);
        //    graphics.DrawString(fileName, new System.Drawing.Font("Arial", 14), System.Drawing.Brushes.Black, new PointF(10, 40));

        //    bmp.Save(imagePath, System.Drawing.Imaging.ImageFormat.Bmp);

        //    // return relative path to front (WPF)
        //    return "/images/" + bmpFileName;
        //}
    }
}
