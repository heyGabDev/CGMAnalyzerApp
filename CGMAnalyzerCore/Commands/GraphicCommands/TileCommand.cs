using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class TileCommand : BaseCgmCommand
    {
        public Point2D Position { get; private set; }
        public byte[] TileData { get; private set; }

        public TileCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Position = argReader.MakePoint(ec, eid);

            var remainingArgs = command.Args.Length - argReader.SizeOfPoint();
            TileData = new byte[remainingArgs];
            for (int i = 0; i < remainingArgs; i++)
            {
                TileData[i] = (byte)argReader.MakeUInt8();
            }
        }

        public override void Draw(Graphics g, Pen pen)
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

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"TILE at {Position}";
    }

}
