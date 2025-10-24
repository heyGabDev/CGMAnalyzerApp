using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Test
{
    // DEBUG 
    public class CgmParserDiagnostic
    {
        public static void DumpCommandSequence(string cgmPath)
        {
            var parser = new CgmParser();
            using var fs = File.OpenRead(cgmPath);

            var reader = new BinaryReader(fs);
            var commands = new List<string>();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                try
                {
                    byte b1 = reader.ReadByte();
                    byte b2 = reader.ReadByte();
                    int k = (b1 << 8) | b2;

                    int ec = (k >> 12) & 0xF;
                    int eid = (k >> 5) & 0x7F;
                    int l = k & 0x1F;

                    commands.Add($"EC={ec}, EID={eid}, L={l}");

                    // Sauter les args
                    if (l != 31)
                    {
                        reader.BaseStream.Seek(l + (l % 2), SeekOrigin.Current);
                    }
                    else
                    {
                        // Forme longue
                        int realLength = reader.ReadByte() << 8 | reader.ReadByte();
                        realLength &= 0x7FFF;
                        reader.BaseStream.Seek(realLength + (realLength % 2), SeekOrigin.Current);
                    }
                }
                catch
                {
                    break;
                }
            }

            // Sauvegarder dans un fichier texte
            File.WriteAllLines($"{cgmPath}.dump.txt", commands);

            // Statistiques
            var stats = commands
                .GroupBy(c => c)
                .Select(g => $"{g.Key}: {g.Count()}x")
                .OrderByDescending(s => int.Parse(s.Split(':')[1].TrimEnd('x')));

            File.WriteAllLines($"{cgmPath}.stats.txt", stats);
        }
    }
}
