using CGMAnalyzerCore.Modeles;

namespace CGMAnalyzer.API.Services.Interfaces
{
    public interface ICgmImageService
    {
        /// <summary>
        /// Traite un fichier CGM complet : parsing + conversion BMP + extraction layers
        /// </summary>
        Task<CgmProcessResult> ProcessCgmFileAsync(IFormFile file);

        /// <summary>
        /// Génère et sauvegarde uniquement le BMP (compatibilité avec l'ancien code)
        /// </summary>
        Task<string> GenerateAndSaveBmpAsync(IFormFile file);

        /// <summary>
        /// Récupère les informations d'un fichier CGM traité
        /// </summary>
        Task<CgmFileInfo> GetCgmInfoAsync(string fileName);

        /// <summary>
        /// Nettoie le cache des traitements anciens
        /// </summary>
        void ClearOldCacheEntries(TimeSpan maxAge);
    }
}
