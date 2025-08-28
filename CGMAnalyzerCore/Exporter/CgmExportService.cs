using CGMAnalyzerCore.Converter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Exporter
{
    public class CgmExportService
    {
        private readonly ILogger<CgmExportService> _logger;

        public CgmExportService(ILogger<CgmExportService> logger)
        {
            _logger = logger;
        }

        public string ExportCgmFile(byte[] buffer, string outputFolder, string fileName)
        {
            return ExportCgmFileAsync(buffer, outputFolder, fileName).GetAwaiter().GetResult();
        }

        public async Task<string> ExportCgmFileAsync(byte[] buffer, string outputFolder, string fileName)
        {
            if (buffer?.Length == 0)
                throw new ArgumentException("Buffer CGM vide", nameof(buffer));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.cgm";

            if (string.IsNullOrWhiteSpace(outputFolder))
                outputFolder = "Export";

            var fullOutputPath = Path.GetFullPath(outputFolder);
            Directory.CreateDirectory(fullOutputPath);

            var filePath = Path.Combine(fullOutputPath, fileName);

            try
            {
                await File.WriteAllBytesAsync(filePath, buffer);

                // Utiliser Console.WriteLine au lieu du logger
                Console.WriteLine($"Fichier CGM exporté: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur export CGM: {ex.Message}");
                throw new IOException($"Impossible d'exporter: {ex.Message}", ex);
            }
        }

    }
}
