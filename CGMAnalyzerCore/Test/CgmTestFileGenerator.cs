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

            // BEGIN METAFILE (0, 1)
            WriteCommand(writer, 0, 1, new byte[] { });

            // METAFILE VERSION (1, 1) = 1
            WriteCommand(writer, 1, 1, new byte[] { 0x00, 0x01 });

            // VDC TYPE (1, 3) = INTEGER
            WriteCommand(writer, 1, 3, new byte[] { 0x00, 0x00 });

            // INTEGER PRECISION (1, 4) = 16 bits
            WriteCommand(writer, 1, 4, new byte[] { 0x00, 0x10 });

            // VDC INTEGER PRECISION (3, 1) = 16 bits
            WriteCommand(writer, 3, 1, new byte[] { 0x00, 0x10 });

            // COLOR PRECISION (1, 7) = 8 bits
            WriteCommand(writer, 1, 7, new byte[] { 0x00, 0x08 });

            // COLOR INDEX PRECISION (1, 8) = 8 bits
            WriteCommand(writer, 1, 8, new byte[] { 0x00, 0x08 });

            // ✅ COLOR SELECTION MODE (2, 2) = DIRECT (0x0001)
            WriteCommand(writer, 2, 2, new byte[] { 0x00, 0x01 });

            // VDC EXTENT (2, 6) = (0,0) to (1000,1000)
            WriteCommand(writer, 2, 6, new byte[] {
                0x00, 0x00, 0x00, 0x00,  // (0, 0)
                0x03, 0xE8, 0x03, 0xE8   // (1000, 1000)
            });

            // BEGIN PICTURE (0, 3)
            WriteCommand(writer, 0, 3, new byte[] { });

            // BEGIN PICTURE BODY (0, 4)
            WriteCommand(writer, 0, 4, new byte[] { });

            // INTERIOR STYLE (5, 22) = SOLID (1)
            WriteCommand(writer, 5, 22, new byte[] { 0x00, 0x01 });

            // ✅ FILL COLOR (5, 23) = ROUGE en DIRECT (255, 0, 0)
            WriteCommand(writer, 5, 23, new byte[] {
                0xFF,  // R = 255 (rouge)
                0x00,  // G = 0
                0x00   // B = 0
            });

            // EDGE VISIBILITY (5, 30) = ON (1)
            WriteCommand(writer, 5, 30, new byte[] { 0x00, 0x01 });

            // ✅ LINE COLOR (5, 4) = NOIR en DIRECT (0, 0, 0)
            WriteCommand(writer, 5, 4, new byte[] {
                0x00,  // R = 0
                0x00,  // G = 0
                0x00   // B = 0
            });

            // POLYGON (4, 7) - Triangle
            WriteCommand(writer, 4, 7, new byte[] {
                0x00, 0xC8,  // X1 = 200
                0x00, 0xC8,  // Y1 = 200
                0x03, 0x20,  // X2 = 800
                0x00, 0xC8,  // Y2 = 200
                0x01, 0xF4,  // X3 = 500
                0x03, 0x20   // Y3 = 800
            });

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
