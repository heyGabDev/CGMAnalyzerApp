using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter
{
    public class CgmExporter
    {
        public static string WriteFile(byte[] buffer, string outputDir, string fileName) { 
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException("Le tampon de données est vide ou nul.");
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Le répertoire de sortie ne peut pas être vide.");
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Le nom de fichier ne peut pas être vide.");
            
            string fullPath = System.IO.Path.Combine(outputDir, fileName);
            System.IO.File.WriteAllBytes(fullPath, buffer);
            return fullPath;
        }

    }
}
