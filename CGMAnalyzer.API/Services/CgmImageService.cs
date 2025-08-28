using CGMAnalyzer.API.Services.Interfaces;
using CGMAnalyzerCore.Converter.Interface;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace CGMAnalyzer.API.Services
{
    public class CgmImageService : ICgmImageService
    {
        private readonly ICgmConverter _converter;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CgmImageService> _logger;

        // Cache pour éviter de retraiter les mêmes fichiers
        private static readonly ConcurrentDictionary<string, CacheEntry> _processCache = new();

        public CgmImageService(ICgmConverter converter, IWebHostEnvironment env, ILogger<CgmImageService> logger)
        {
            _converter = converter;
            _env = env;
            _logger = logger;

            // S'assurer que le dossier images existe
            var imagesPath = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(imagesPath);
        }

        // Ta méthode existante - améliorée avec cache
        public async Task<string> GenerateAndSaveBmpAsync(IFormFile file)
        {
            var fileHash = await CalculateFileHashAsync(file);

            // Vérifier le cache
            if (_processCache.TryGetValue(fileHash, out var cachedEntry))
            {
                var cachedPath = Path.Combine(_env.WebRootPath, "images", Path.GetFileName(cachedEntry.BmpPath));
                if (File.Exists(cachedPath))
                {
                    _logger.LogInformation("Cache hit pour {FileName}", file.FileName);
                    return cachedEntry.BmpPath;
                }
                else
                {
                    // Fichier en cache supprimé, retirer de la cache
                    _processCache.TryRemove(fileHash, out _);
                }
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var bmpFileName = $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bmp";
            var fullPath = Path.Combine(_env.WebRootPath, "images", bmpFileName);
            var bmpPath = "/images/" + bmpFileName;

            try
            {
                using var stream = file.OpenReadStream();
                var bmpBytes = await _converter.ConvertToBmpBytesAsync(stream, file.FileName);

                await File.WriteAllBytesAsync(fullPath, bmpBytes);

                // Ajouter au cache
                _processCache.TryAdd(fileHash, new CacheEntry
                {
                    BmpPath = bmpPath,
                    CreatedAt = DateTime.UtcNow,
                    OriginalFileName = file.FileName
                });

                _logger.LogInformation("Fichier traité et mis en cache: {FileName} -> {BmpPath}", file.FileName, bmpPath);

                return bmpPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de {FileName}", file.FileName);

                // Nettoyer le fichier partiellement créé
                if (File.Exists(fullPath))
                {
                    try
                    {
                        File.Delete(fullPath);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogWarning(deleteEx, "Impossible de supprimer le fichier temporaire {FullPath}", fullPath);
                    }
                }

                throw;
            }
        }

        // Nouvelle méthode pour le nettoyage du cache
        public void ClearOldCacheEntries(TimeSpan maxAge)
        {
            var cutoff = DateTime.UtcNow - maxAge;
            var keysToRemove = new List<string>();
            var filesDeleted = 0;
            var cacheEntriesRemoved = 0;

            foreach (var kvp in _processCache)
            {
                if (kvp.Value.CreatedAt < cutoff)
                {
                    keysToRemove.Add(kvp.Key);

                    // Essayer de supprimer le fichier physique
                    try
                    {
                        var physicalPath = Path.Combine(_env.WebRootPath, "images", Path.GetFileName(kvp.Value.BmpPath));
                        if (File.Exists(physicalPath))
                        {
                            File.Delete(physicalPath);
                            filesDeleted++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de supprimer le fichier obsolète {BmpPath}", kvp.Value.BmpPath);
                    }
                }
            }

            // Supprimer les entrées du cache
            foreach (var key in keysToRemove)
            {
                if (_processCache.TryRemove(key, out _))
                {
                    cacheEntriesRemoved++;
                }
            }

            _logger.LogInformation("Nettoyage du cache terminé: {CacheEntriesRemoved} entrées supprimées, {FilesDeleted} fichiers supprimés",
                cacheEntriesRemoved, filesDeleted);
        }

        // Méthodes utilitaires privées
        private async Task<string> CalculateFileHashAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var sha256 = SHA256.Create();
                var hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Impossible de calculer le hash pour {FileName}, utilisation du nom de fichier", file.FileName);
                // Fallback sur nom de fichier + taille
                return $"{file.FileName}_{file.Length}";
            }
        }

        // Méthodes publiques utiles pour le monitoring
        public int GetCacheSize()
        {
            return _processCache.Count;
        }

        public void ClearAllCache()
        {
            var count = _processCache.Count;
            _processCache.Clear();
            _logger.LogInformation("Cache complètement vidé: {Count} entrées supprimées", count);
        }

        // Informations sur un fichier en cache
        public bool IsFileInCache(IFormFile file)
        {
            var fileHash = CalculateFileHashAsync(file).GetAwaiter().GetResult();
            return _processCache.ContainsKey(fileHash);
        }
    }

    // Classe pour les entrées de cache
    internal class CacheEntry
    {
        public string BmpPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
    }
}