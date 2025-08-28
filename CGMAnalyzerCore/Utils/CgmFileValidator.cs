using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Utils
{
    public static class CgmFileValidator
    {
        private static readonly string[] ValidExtensions = { ".cgm", ".cgm.gz", ".cgmz" };
        private const int MinFileSize = 8; // bytes minimum
        private const int MaxFileSize = 100 * 1024 * 1024; // 100MB max

        public static ValidationResult ValidateFile(IFormFile file)
        {
            var result = new ValidationResult { IsValid = true };

            // Vérifier l'extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ValidExtensions.Contains(extension))
            {
                result.IsValid = false;
                result.Errors.Add($"Extension non supportée: {extension}. Extensions valides: {string.Join(", ", ValidExtensions)}");
            }

            // Vérifier la taille
            if (file.Length < MinFileSize)
            {
                result.IsValid = false;
                result.Errors.Add($"Fichier trop petit: {file.Length} bytes (minimum: {MinFileSize})");
            }
            else if (file.Length > MaxFileSize)
            {
                result.IsValid = false;
                result.Errors.Add($"Fichier trop volumineux: {file.Length / 1024 / 1024}MB (maximum: {MaxFileSize / 1024 / 1024}MB)");
            }

            // Vérifier le nom de fichier
            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                result.IsValid = false;
                result.Errors.Add("Nom de fichier vide");
            }
            else if (file.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                result.IsValid = false;
                result.Errors.Add("Nom de fichier contient des caractères invalides");
            }

            return result;
        }

        public static async Task<ValidationResult> ValidateFileContentAsync(IFormFile file)
        {
            var result = ValidateFile(file);
            if (!result.IsValid) return result;

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new BinaryReader(stream);

                // Vérifier la signature CGM basique
                if (stream.Length >= 2)
                {
                    var firstWord = reader.ReadUInt16();
                    var elementClass = (firstWord >> 12) & 0x0F;
                    var elementId = (firstWord >> 5) & 0x7F;

                    // Classes attendues au début d'un fichier CGM
                    if (elementClass != 0 && elementClass != 1)
                    {
                        result.IsValid = false;
                        result.Errors.Add($"Structure CGM invalide. Première commande: Classe={elementClass}, ID={elementId}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Erreur lors de la validation du contenu: {ex.Message}");
            }

            return result;
        }
    }
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}