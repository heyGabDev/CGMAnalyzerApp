using CGMAnalyzerCore.Converter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter
{
    public class CgmSplitter
    {
        /// <summary>
        /// Permet d’extraire une portion binaire d’un fichier CGM entre deux positions
        /// Transmet les données binaires à un extracteur
        /// </summary>
        /// <param name="extractor"></param>
        /// <param name="fileStream"></param>
        /// <param name="startPosition"></param>
        /// <param name="currentPosition"></param>
        /// <param name="fileName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        public static void DumpToStream(
                ICgmExtractor extractor,
                FileStream fileStream,
                long startPosition,
                long currentPosition,
                string fileName)
            {
                if (startPosition < 0 || currentPosition <= startPosition || currentPosition > fileStream.Length)
                    throw new ArgumentOutOfRangeException("Invalid start or end position.");

                long length = currentPosition - startPosition;
                byte[] buffer = new byte[length];

                fileStream.Seek(startPosition, SeekOrigin.Begin);
                int read = fileStream.Read(buffer, 0, (int)length);

                if (read != length)
                    throw new IOException("Could not read full segment from file.");

                extractor.ExtractSegment(fileName, buffer);
            }

        /// <summary>
        /// extrait une portion binaire d’un fichier CGM et l’écrit directement sur le disque
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="fileNameExtractor"></param>
        /// <param name="fileStream"></param>
        /// <param name="startPosition"></param>
        /// <param name="currentPosition"></param>
        /// <param name="currentFileName"></param>
        /// <param name="index"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        public static void DumpToFile(
                string outputDir,
                IBeginMetafileNameExtractor fileNameExtractor,
                FileStream fileStream,
                long startPosition,
                long currentPosition,
                string currentFileName,
                int index)
        {
            if (startPosition < 0 || currentPosition <= startPosition || currentPosition > fileStream.Length)
                throw new ArgumentOutOfRangeException("Invalid start or end position.");

            long length = currentPosition - startPosition;
            byte[] buffer = new byte[length];

            fileStream.Seek(startPosition, SeekOrigin.Begin);
            int read = fileStream.Read(buffer, 0, (int)length);

            if (read != length)
                throw new IOException("Could not read full segment from file.");

            // Obtenir le nom de fichier à partir de l'extracteur
            string outputFileName = fileNameExtractor.GetFileName(buffer, index);

            string outputPath = Path.Combine(outputDir, outputFileName);
            File.WriteAllBytes(outputPath, buffer);
        }


        /// <summary>
        /// Permet de diviser un fichier CGM en plusieurs segments, en créant un fichier pour chaque segment.
        /// </summary>
        /// <param name="cgmFilePath"></param>
        /// <param name="outputDir"></param>
        /// <param name="fileNameExtractor"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void Split(string cgmFilePath, ICgmExtractor extractor)
        {
            if (!File.Exists(cgmFilePath))
                throw new FileNotFoundException("Fichier CGM introuvable", cgmFilePath);

            using var fileStream = new FileStream(cgmFilePath, FileMode.Open, FileAccess.Read);
            long fileLength = fileStream.Length;

            long start = 0;
            int index = 1;

            for (long pos = 0; pos < fileLength;)
            {
                byte[] header = new byte[2];
                fileStream.Seek(pos, SeekOrigin.Begin);
                fileStream.Read(header, 0, 2);

                int ec = (header[0] & 0b11111000) >> 3;
                int eid = ((header[0] & 0b00000111) << 5) | ((header[1] & 0b11111000) >> 3);

                // BeginMetafile command
                if (ec == 0 && eid == 1 && pos != start)
                {
                    long current = pos;
                    DumpToStream(extractor, fileStream, start, current, $"segment_{index++}.cgm");
                    start = current;
                }

                pos += 2;
            }

            // Dernier segment
            if (start < fileLength)
            {
                DumpToStream(extractor, fileStream, start, fileLength, $"segment_{index}.cgm");
            }
        }


        public static void Split(
        string cgmFilePath,
        string outputDir,
        IBeginMetafileNameExtractor fileNameExtractor)
        {
            if (!File.Exists(cgmFilePath))
                throw new FileNotFoundException("CGM file not found.", cgmFilePath);

            Directory.CreateDirectory(outputDir); // Assure que le dossier existe

            using var fileStream = new FileStream(cgmFilePath, FileMode.Open, FileAccess.Read);
            long fileLength = fileStream.Length;

            long start = 0;
            int index = 1;

            for (long pos = 0; pos < fileLength;)
            {
                byte[] header = new byte[2];
                fileStream.Seek(pos, SeekOrigin.Begin);
                fileStream.Read(header, 0, 2);

                int ec = (header[0] & 0b11111000) >> 3;
                int eid = ((header[0] & 0b00000111) << 5) | ((header[1] & 0b11111000) >> 3);

                // Détection BeginMetafile : class 0, id 1
                if (ec == 0 && eid == 1 && pos != start)
                {
                    long current = pos;
                    DumpToFile(outputDir, fileNameExtractor, fileStream, start, current, cgmFilePath, index++);
                    start = current;
                }

                // Avance minimale (optimisable)
                pos += 2;
            }

            // Dernier bloc
            if (start < fileLength)
            {
                DumpToFile(outputDir, fileNameExtractor, fileStream, start, fileLength, cgmFilePath, index);
            }
        }


    }
}
