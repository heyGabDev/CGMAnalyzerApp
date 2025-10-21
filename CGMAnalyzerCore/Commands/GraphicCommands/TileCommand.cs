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
    /// <summary>
    /// TILE (case 29) - Tuile couleur
    /// </summary>
    public class TileCommand : BaseCgmCommand
    {
        public Point2D Position { get; private set; } = new Point2D(0, 0);
        public byte[] TileData { get; private set; } = Array.Empty<byte>();

        public TileCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TileCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                Position = argReader.MakePoint();
                Debug.WriteLine($"[TileCommand] Read Position=({Position.X}, {Position.Y})");

                int remainingBytes = this.RemainingArgs();
                TileData = new byte[remainingBytes];

                for (int i = 0; i < remainingBytes; i++)
                {
                    TileData[i] = (byte)argReader.MakeUInt8();
                }
                Debug.WriteLine($"[TileCommand] Read TileData Length={TileData.Length}");
                ValidateArgumentsRead("TileCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TileCommand ERROR] {ex.Message}");
                Position = new Point2D(0, 0);
                TileData = Array.Empty<byte>();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (TileData == null || TileData.Length == 0)
            {
                Debug.WriteLine("[TileCommand] No tile data to draw.");
                return;
            }

            try
            {
                // Implémentation similaire à BitonalTile mais avec couleurs
                const int tileSize = 8;

                for (int i = 0; i < Math.Min(TileData.Length, 64); i++)
                {
                    var x = Position.X + (i % tileSize);
                    var y = Position.Y + (i / tileSize);
                    var colorValue = TileData[i];

                    using var brush = new SolidBrush(Color.FromArgb(colorValue, colorValue, colorValue));
                    g.FillRectangle(brush, (float)x, (float)y, 1, 1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TileCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"TILE at ({Position.X}, {Position.Y}) - {TileData.Length} bytes";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
