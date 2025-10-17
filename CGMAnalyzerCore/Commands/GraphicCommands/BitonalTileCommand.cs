using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class BitonalTileCommand : BaseCgmCommand
    {
        public Point2D Position { get; private set; } = new Point2D(0, 0);
        public byte[] TileData { get; private set; } = Array.Empty<byte>();

        public BitonalTileCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[BitonalTileCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Position = argReader.MakePoint();
                Debug.WriteLine($"[BitonalTileCommand] Read Position=({Position.X}, {Position.Y})");

                var remainingArgs = command.Args.Length - argReader.SizeOfPoint();
                TileData = new byte[remainingArgs];
                for (int i = 0; i < remainingArgs; i++)
                {
                    TileData[i] = (byte)argReader.MakeUInt8();
                }
                Debug.WriteLine($"[BitonalTileCommand] Read TileData Length={TileData.Length}");
                ValidateArgumentsRead("BitonalTileCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BitonalTileCommand ERROR] {ex.Message}");
                Position = new Point2D(0, 0);
                TileData = Array.Empty<byte>();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (TileData == null || TileData.Length == 0)
            {
                Debug.WriteLine("[BitonalTileCommand] No tile data to draw.");
                return;
            }

            try
            {
                // Dessiner une image monochrome simple
                const int tileSize = 8;
                using var brush = new SolidBrush(pen.Color);

                for (int i = 0; i < Math.Min(TileData.Length, 64); i++)
                {
                    var x = Position.X + (i % tileSize);
                    var y = Position.Y + (i / tileSize);

                    if (TileData[i] > 0)
                    {
                        g.FillRectangle(brush, (float)x, (float)y, 1, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BitonalTileCommand ERROR] Error during drawing: {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"BITONAL_TILE at {Position}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
