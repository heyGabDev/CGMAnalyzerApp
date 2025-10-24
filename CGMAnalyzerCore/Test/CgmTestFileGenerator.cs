using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Test
{
    // DEBUG 
    public class CgmTestFileGenerator
    {
        public static void CreateSimplePolygonCGM(string outputPath)
        {
            using var fs = new FileStream(outputPath, FileMode.Create);
            using var writer = new BinaryWriter(fs);

            // === ENTÊTE CGM ===

            // BEGIN METAFILE (0, 1) - nom vide
            WriteCommand(writer, 0, 1, new byte[] { 0x00 });

            // METAFILE VERSION (1, 1) - version 1
            WriteCommand(writer, 1, 1, new byte[] { 0x00, 0x01 });

            // VDC TYPE (1, 3) - INTEGER
            WriteCommand(writer, 1, 3, new byte[] { 0x00, 0x00 });

            // INTEGER PRECISION (1, 4) - 16 bits
            WriteCommand(writer, 1, 4, new byte[] { 0x00, 0x10 });

            // COLOR PRECISION (1, 7) - 8 bits
            WriteCommand(writer, 1, 7, new byte[] { 0x00, 0x08 });

            // COLOR SELECTION MODE (2, 2) - INDEXED
            WriteCommand(writer, 2, 2, new byte[] { 0x00, 0x01 });

            // VDC EXTENT (2, 6) - (0,0) to (1000,1000)
            WriteCommand(writer, 2, 6, new byte[] {
            0x00, 0x00, 0x00, 0x00,  // x1, y1
            0x03, 0xE8, 0x03, 0xE8   // x2, y2 (1000, 1000)
            });

            // === PICTURE ===

            // BEGIN PICTURE (0, 3)
            WriteCommand(writer, 0, 3, new byte[] { 0x00 });

            // BEGIN PICTURE BODY (0, 4)
            WriteCommand(writer, 0, 4, new byte[] { });

            // === ATTRIBUTS ===

            // INTERIOR STYLE (5, 22) - SOLID (1)
            WriteCommand(writer, 5, 22, new byte[] { 0x00, 0x01 });

            // FILL COLOR (5, 23) - RED (index 1 = red dans table par défaut)
            WriteCommand(writer, 5, 23, new byte[] { 0x01 });

            // EDGE VISIBILITY (5, 30) - ON (1)
            WriteCommand(writer, 5, 30, new byte[] { 0x00, 0x01 });

            // LINE COLOR (5, 4) - BLACK (index 0)
            WriteCommand(writer, 5, 4, new byte[] { 0x00 });

            // === POLYGON ===

            // POLYGON (4, 7) - Triangle
            WriteCommand(writer, 4, 7, new byte[] {
            0x01, 0x00, 0x01, 0x00,  // Point 1: (256, 256)
            0x03, 0x00, 0x01, 0x00,  // Point 2: (768, 256)
            0x02, 0x00, 0x03, 0x00   // Point 3: (512, 768)
        });

            // === FIN ===

            // END PICTURE (0, 5)
            WriteCommand(writer, 0, 5, new byte[] { });

            // END METAFILE (0, 2)
            WriteCommand(writer, 0, 2, new byte[] { });
        }

        private static void WriteCommand(BinaryWriter writer, int elementClass, int elementId, byte[] args)
        {
            // Format: CCCC EEEEEEE LLLLL (16 bits)
            // C = class (4 bits), E = element ID (7 bits), L = length (5 bits)

            int length = args.Length;
            bool longForm = length > 30;

            if (!longForm)
            {
                // FORME COURTE
                int header = (elementClass << 12) | (elementId << 5) | length;
                writer.Write((byte)(header >> 8));
                writer.Write((byte)(header & 0xFF));
                writer.Write(args);

                // Padding si impair
                if (length % 2 == 1)
                    writer.Write((byte)0x00);
            }
            else
            {
                // FORME LONGUE (length = 31)
                int header = (elementClass << 12) | (elementId << 5) | 31;
                writer.Write((byte)(header >> 8));
                writer.Write((byte)(header & 0xFF));

                // Longueur réelle (bit 15 = 0 pour fin)
                writer.Write((byte)(length >> 8));
                writer.Write((byte)(length & 0xFF));
                writer.Write(args);

                // Padding si impair
                if (length % 2 == 1)
                    writer.Write((byte)0x00);
            }
        }
    }
}
