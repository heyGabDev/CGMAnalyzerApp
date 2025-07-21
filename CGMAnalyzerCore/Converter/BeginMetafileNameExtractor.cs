using System;
using System.IO;
using System.Text;
using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Parser;

public class BeginMetafileNameExtractor : IBeginMetafileNameExtractor
{
    public string GetFileName(byte[] data, int index)
    {
        try
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            // Lire les 2 premiers octets (header)
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();

            int ec = (b1 & 0b11111000) >> 3;
            int eid = ((b1 & 0b00000111) << 5) | ((b2 & 0b11111000) >> 3);
            int length = ((b2 & 0b00000111) << 8) | reader.ReadByte();

            if (ec == 0 && eid == 1) // BeginMetafile
            {
                // Lire la chaîne de nom (souvent codée en ASCII avec longueur)
                int nameLength = reader.ReadByte(); // longueur du nom
                byte[] nameBytes = reader.ReadBytes(nameLength);
                string name = Encoding.ASCII.GetString(nameBytes);

                // Nettoyer pour nom de fichier
                name = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
                return $"{name}_{index}.cgm";
            }
        }
        catch
        {
            // fallback si échec de lecture
        }

        return $"part_{index}.cgm";
    }
}
