using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class CellArrayCommand : BaseCgmCommand
    {
        public Point2D Corner1 { get; private set; }
        public Point2D Corner2 { get; private set; }
        public Point2D Corner3 { get; private set; }
        public int Nx { get; private set; }
        public int Ny { get; private set; }
        public byte[] CellData { get; private set; }

        public CellArrayCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Corner1 = argReader.MakePoint(ec, eid);
            Corner2 = argReader.MakePoint(ec, eid);
            Corner3 = argReader.MakePoint(ec, eid);

            Nx = argReader.MakeInt();
            Ny = argReader.MakeInt();

            // Le reste des données représente les cellules de couleur
            var remainingArgs = command.Args.Length - (3 * argReader.SizeOfPoint() + 2 * argReader.SizeOfInt());
            CellData = new byte[remainingArgs];
            for (int i = 0; i < remainingArgs; i++)
            {
                CellData[i] = (byte)argReader.MakeUInt8();
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Implémentation simplifiée d'un tableau de cellules (image bitmap)
            if (Nx <= 0 || Ny <= 0) return;

            var cellWidth = Math.Abs(Corner2.X - Corner1.X) / Nx;
            var cellHeight = Math.Abs(Corner3.Y - Corner1.Y) / Ny;

            for (int y = 0; y < Math.Min(Ny, 50); y++) // Limiter pour performance
            {
                for (int x = 0; x < Math.Min(Nx, 50); x++)
                {
                    var cellIndex = y * Nx + x;
                    if (cellIndex < CellData.Length)
                    {
                        var colorValue = CellData[cellIndex];
                        var cellColor = Color.FromArgb(colorValue, colorValue, colorValue);

                        using var brush = new SolidBrush(cellColor);
                        g.FillRectangle(brush,
                            (float)(Corner1.X + x * cellWidth),
                            (float)(Corner1.Y + y * cellHeight),
                            (float)cellWidth,
                            (float)cellHeight);
                    }
                }
            }
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"CELL_ARRAY {Nx}x{Ny}";
    }
}
