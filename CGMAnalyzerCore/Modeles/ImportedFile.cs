using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Models
{
    public static class AppPaths
    {
        public static string SolutionRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
        public static string ApiImagePath => Path.Combine(SolutionRoot, "CGMAnalyzer.API", "wwwroot", "images");
    }
    public class ImportedFile
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string BmpPath { get; set; } // ex: /images/image.bmp
        public List<string> Layers { get; set; } = new();
        public override string ToString() => FileName; // Ce qui sera affiché dans la ListBox

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFullPath()
        {
            return Path.Combine(AppPaths.ApiImagePath, FileName.TrimStart());
        }

        /// <summary>
        /// Renvoie le chemin relatif utilisable dans un navigateur (/images/image.bmp)
        /// </summary>
        /// <returns></returns>
        public string GetWebPath()
        {
            // Si BmpPath est déjà relatif, retourne-le
            if (!string.IsNullOrEmpty(BmpPath) && BmpPath.StartsWith("/"))
                return BmpPath;

            // Sinon, le reconstruit à partir de FileName
            return "/images/" + FileName.Replace("\\", "/");
        }
    }

 }
