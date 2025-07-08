using CGMAnalyzer.API.Services;
using CGMAnalyzerCore.Modeles;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CGMAnalyzer.API.Controllers;

[ApiController]
[Route("api/cgm")]
public class CGMController : ControllerBase
{
    [HttpPost("import")]
    public async Task<IActionResult> ImportCgmFile(
        List<IFormFile> files, 
        [FromServices] ICgmConverter cgmConverter, 
        [FromServices] ICGMLayerDetector cgmLayerDetector)
    {
        try
        {
            if (files == null || files.Count == 0)
            return BadRequest($"Aucun fichier reçu.");

            var results = new List<CGMResult>();

            foreach (var file in files)
            {
                // management layers - temp save
                var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                // analyze layers
                var layerOffsets = cgmLayerDetector.DetectLayers(tempPath);
                var layerNames = layerOffsets.Select((_, i) => $"Layer {i + 1}").ToList();

                // BMP conversion
                var bmpPath = await cgmConverter.ConvertToBmpAsync(file);
                var result = new CGMResult
                {
                    FileName = file.FileName,
                    BmpPath = bmpPath,
                    Errors = new List<string> { $"Pas d'erreur" },
                    Layers = layerNames,
                };
                results.Add(result);
                System.IO.File.Delete(tempPath);
            }
            return Ok(results); // Pour test (un fichier à la fois)

        }
        catch (Exception err)
        {
            //Logg err
            Debug.WriteLine($"Import error : {err.Message}");
            return StatusCode(500, $"Servor error : {err.Message}");
        }

        
    }
}
