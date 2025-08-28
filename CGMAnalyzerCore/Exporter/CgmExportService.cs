using CGMAnalyzerCore.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Exporter
{
    public class CgmExportService
    {
        private readonly ILogger<CgmExportService>? _logger;

        public CgmExportService(ILogger<CgmExportService>? logger = null)
        {
            _logger = logger;
        }

        public string ExportCgmFile(byte[] buffer, string outputDir, string fileName)
        {
            return ExportCgmFileAsync(buffer, outputFolder, fileName).GetAwaiter().GetResult();

            // TODO SGC - DELETE
            //return CgmExporter.WriteFile(buffer, outputDir, fileName);
        }

        public async Task<string> ExportCgmFileAsync(byte[] buffer, string outputFolder, string fileName)
        {
            if (buffer?.Length == 0)
                throw new ArgumentException("Buffer CGM vide", nameof(buffer));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.cgm";

            if (string.IsNullOrWhiteSpace(outputFolder))
                outputFolder = "Export";

            // Créer le dossier de destination
            var fullOutputPath = Path.GetFullPath(outputFolder);
            Directory.CreateDirectory(fullOutputPath);

            // Chemin complet du fichier
            var filePath = Path.Combine(fullOutputPath, fileName);

            try
            {
                // Écriture asynchrone du fichier
                await File.WriteAllBytesAsync(filePath, buffer);

                _logger?.LogInformation("Fichier CGM exporté avec succès: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erreur lors de l'export du fichier CGM: {FilePath}", filePath);
                throw new IOException($"Impossible d'exporter le fichier CGM: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exporte avec validation du contenu CGM
        /// </summary>
        public async Task<ExportResult> ExportCgmWithValidationAsync(byte[] buffer, string outputFolder, string fileName)
        {
            var result = new ExportResult { FileName = fileName };

            try
            {
                // Validation basique du contenu CGM
                if (!IsValidCgmContent(buffer))
                {
                    result.Success = false;
                    result.ErrorMessage = "Le contenu ne semble pas être un fichier CGM valide";
                    return result;
                }

                var filePath = await ExportCgmFileAsync(buffer, outputFolder, fileName);

                result.Success = true;
                result.FilePath = filePath;
                result.FileSize = buffer.Length;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        private bool IsValidCgmContent(byte[] buffer)
        {
            if (buffer?.Length < 8) return false;

            // Vérification basique : un fichier CGM commence généralement par
            // des commandes de métafile (classe 0 ou 1)
            try
            {
                using var stream = new MemoryStream(buffer);
                using var reader = new BinaryReader(stream);

                // Lire les premiers bytes pour détecter la structure CGM
                var firstWord = reader.ReadUInt16();
                var elementClass = (firstWord >> 12) & 0x0F;

                // Les classes 0 (Delimiter) et 1 (Metafile Descriptor) sont attendues au début
                return elementClass == 0 || elementClass == 1;
            }
            catch
            {
                return false;
            }
        }

        public class ExportResult
        {
            public string FileName { get; set; } = string.Empty;
            public string? FilePath { get; set; }
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public long FileSize { get; set; }
            public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
        }
    }
}
