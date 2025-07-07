using CGMAnalyzer.API.Modeles;
using CGMAnalyzer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CGMAnalyzer.API.Controllers;

[ApiController]
[Route("api/cgm")]
public class CGMController : ControllerBase
{
    [HttpPost("import")]
    public async Task<IActionResult> ImportCgmFile(List<IFormFile> files, [FromServices]ICgmConverter cgmConverter)
    {
        try
        {
            if (files == null || files.Count == 0)
            return BadRequest($"Aucun fichier reçu.");

            var results = new List<CGMResult>();

            foreach (var file in files)
            {
                var bmpPath = await cgmConverter.ConvertToBmpAsync(file);
                var result = new CGMResult
                {
                    FileName = file.FileName,
                    BmpPath = bmpPath,
                    Errors = new List<string> { $"Pas d'erreur" }
                };
                results.Add(result);
            }
            return Ok(results.First()); // Pour test (un fichier à la fois)
        }
        catch (Exception err)
        {
            //Logg err
            Console.WriteLine($"Import error : {err.Message}");
            return StatusCode(500, $"Servor error : {err.Message}");
        }

        
    }
}
