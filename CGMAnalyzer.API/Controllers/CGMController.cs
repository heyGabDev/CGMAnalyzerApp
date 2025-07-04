using CGMAnalyzer.API.Modeles;
using Microsoft.AspNetCore.Mvc;

namespace CGMAnalyzer.API.Controllers;

[ApiController]
[Route("api/cgm")]
public class CGMController : ControllerBase
{
    [HttpPost("import")]
    public async Task<IActionResult> Import(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("Aucun fichier reçu.");

        var results = new List<CGMResult>();

        foreach (var file in files)
        {
            // Simuler un traitement
            var result = new CGMResult
            {
                FileName = file.FileName,
                BmpPath = "/images/" + file.FileName + ".bmp",
                Errors = new List<string> { "Erreur fictive pour test" }
            };
            results.Add(result);
        }
        return Ok(results.First()); // Pour test MAUI (un fichier à la fois)
    }
}
