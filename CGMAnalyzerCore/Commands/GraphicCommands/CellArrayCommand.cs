using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class CellArrayCommand : BaseCgmCommand
    {
        public Point2D Corner1 { get; private set; } = new Point2D(0, 0);
        public Point2D Corner2 { get; private set; } = new Point2D(0, 0);
        public Point2D Corner3 { get; private set; } = new Point2D(0, 0);
        public int Nx { get; private set; } = 0;
        public int Ny { get; private set; } = 0;
        public byte[] CellData { get; private set; }

        public CellArrayCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid,l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CellArrayCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                // Lire les coins et les dimensions
                Corner1 = argReader.MakePoint();
                Debug.WriteLine($"[CellArrayCommand] Read Corner1=({Corner1.X}, {Corner1.Y})");

                Corner2 = argReader.MakePoint();
                Debug.WriteLine($"[CellArrayCommand] Read Corner2=({Corner2.X}, {Corner2.Y})");

                Corner3 = argReader.MakePoint();
                Debug.WriteLine($"[CellArrayCommand] Read Corner3=({Corner3.X}, {Corner3.Y})");

                Nx = argReader.MakeInt();
                Debug.WriteLine($"[CellArrayCommand] Read Nx={Nx}");

                Ny = argReader.MakeInt();
                Debug.WriteLine($"[CellArrayCommand] Read Ny={Ny}");

                // Lire les données de cellules (utiliser RemainingArgs())
                int remainingBytes = this.RemainingArgs();
                CellData = new byte[remainingBytes];

                for (int i = 0; i < remainingBytes; i++)
                {
                    CellData[i] = (byte)argReader.MakeUInt8();
                }

                Debug.WriteLine($"[CellArrayCommand] CellData length: {CellData.Length}");
                Debug.WriteLine($"[CellArrayCommand] Expected cells: {Nx * Ny}, Actual bytes: {CellData.Length}");
                ValidateArgumentsRead("CellArrayCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CellArrayCommand ERROR] {ex.Message}");
                Corner1 = new Point2D(0, 0);
                Corner2 = new Point2D(0, 0);
                Corner3 = new Point2D(0, 0);
                Nx = 0;
                Ny = 0;
                CellData = Array.Empty<byte>();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if(CellData == null || CellData.Length == 0)
            {
                Debug.WriteLine("[CellArrayCommand] Pas de données de cellules à dessiner");
                return;
            }

            if(Nx <= 0 || Ny <= 0)
            {
                Debug.WriteLine("[CellArrayCommand] Dimensions invalides Nx ou Ny");
                return;
            }

            Debug.WriteLine("[CellArrayCommand] Dessin du tableau de cellules");

            try
            {
                var cellWidth = Math.Abs(Corner2.X - Corner1.X) / Nx;
                var cellHeight = Math.Abs(Corner3.Y - Corner1.Y) / Ny;

                // Limiter à 50x50 pour éviter les problèmes de performance
                int maxX = Math.Min(Nx, 50);
                int maxY = Math.Min(Ny, 50);

                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[CellArrayCommand DRAW ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"CELL_ARRAY {Nx}x{Ny} ({CellData?.Length ?? 0} bytes)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
