namespace CGMAnalyzer.API.Services
{
    public class CGMLayerDetector : ICGMLayerDetector
    {
        public List<int> DetectLayers(string tempPath)
        {

            if (!File.Exists(tempPath))
            {
                Console.WriteLine("Fichier CGM non trouvé.");
                return new List<int>();
            }

            byte[] cgmData = File.ReadAllBytes(tempPath);

            // Motif supposé pour début de couche (ex: BEGIN PICTURE en CGM binaire peut être 0x20 0x04)
            byte[] layerStartPattern = [0x20, 0x04];

            List<int> layerOffsets = FindPatternOffsets(cgmData, layerStartPattern);

            Console.WriteLine($"Fichier CGM chargé : {tempPath}");
            Console.WriteLine($"Nombre de couches trouvées (motif 0x20 0x04) : {layerOffsets.Count}");
            for (int i = 0; i < layerOffsets.Count; i++)
            {
                Console.WriteLine($"Layer {i + 1} start offset : 0x{layerOffsets[i]:X}");
            }

            // Optionnel : enregistrer chaque couche dans un fichier à part
            if (layerOffsets.Count == 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    int start = layerOffsets[i];
                    int end = (i < 4) ? layerOffsets[i + 1] : cgmData.Length;
                    int length = end - start;

                    byte[] chunk = new byte[length];
                    Array.Copy(cgmData, start, chunk, 0, length);

                    File.WriteAllBytes($"Layer_{i + 1}.cgm", chunk);
                    Console.WriteLine($"➡️  Exporté : Layer_{i + 1}.cgm ({length} octets)");
                }
                
            }
            else
            {
                Console.WriteLine("⚠️ Nombre de couches différent de 5, pas d'extraction automatique.");
            }
            return layerOffsets;
        }


        private List<int> FindPatternOffsets(byte[] data, byte[] pattern)
        {
            List<int> offsets = new List<int>();
            for (int i = 0; i < data.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    offsets.Add(i);
                }
            }
            return offsets;
        }

    }
}
