using CGMAnalyzer.API.Services;
using CGMAnalyzerCore.Modeles;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace CGMAnalyzer.API.Controllers;

[ApiController]
[Route("api/cgm")]
public class CGMController : ControllerBase
{
    private readonly ICgmImageService _imageService;

    public CGMController(ICgmImageService imageService)
    {
        _imageService = imageService;
    }


    [HttpPost("import")]
    public async Task<IActionResult> ImportCgmFile(List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
            return BadRequest($"Aucun fichier reçu.");

            var results = new List<CGMResult>();

            foreach (var file in files)
            {
                // management layers - temp save
                //var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                //using (var stream = new FileStream(tempPath, FileMode.Create))
                //    await file.CopyToAsync(stream);

                // BMP conversion
                var bmpPath = await _imageService.GenerateAndSaveBmpAsync(file);
                var result = new CGMResult
                {
                    FileName = file.FileName,
                    BmpPath = bmpPath,
                    Errors = new List<string> { $"Pas d'erreur" },
                };
                results.Add(result);
                //System.IO.File.Delete(tempPath);
            }
            return Ok(results);

        }
        catch (Exception err)
        {
            //Logg err
            Debug.WriteLine($"Import error : {err.Message}");
            return StatusCode(500, $"Servor error : {err.Message}");
        }
    }
}
