using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class VDCExtentCommand : BaseCgmCommand
    {
        public Point2D.Double LowerLeftCorner { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double UpperRightCorner { get; private set; } = new Point2D.Double(1, 1);

        public VDCExtentCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCExtentCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                // VDC Extent utilise TOUJOURS VDC INTEGER, PAS VDC REAL !
                // VdcType=REAL pour les formes, VDC Extent est en INTEGER
                var argReader = new ExtractedArgumentReader(this);
                int x1, y1, x2, y2;

                // Forcer VDC Extent utilise TOUJOURS 16 - bit, peu importe VdcIntegerPrecision!
                x1 = argReader.MakeSignedInt16();
                y1 = argReader.MakeSignedInt16();
                x2 = argReader.MakeSignedInt16();
                y2 = argReader.MakeSignedInt16();

                LowerLeftCorner = new Point2D.Double(x1, y1);
                UpperRightCorner = new Point2D.Double(x2, y2);

                Debug.WriteLine($"[VDCExtentCommand] LowerLeft=({x1}, {y1}), UpperRight=({x2}, {y2})");

                // Pour les CGM Version 4 (16 bytes) : 4 points ou 2 points en 32-bit
                // Lire les 8 bytes restants (probablement Device Viewport ou padding)
                if (command.RemainingArgs() >= 8)
                {
                    int x3 = argReader.MakeSignedInt16();
                    int y3 = argReader.MakeSignedInt16();
                    int x4 = argReader.MakeSignedInt16();
                    int y4 = argReader.MakeSignedInt16();

                    Debug.WriteLine($"[VDCExtentCommand] Extra values: ({x3}, {y3}), ({x4}, {y4}) - ignorés");
                }

                CgmContext.SetVdcExtent(LowerLeftCorner, UpperRightCorner);

                ValidateArgumentsRead("VDCExtentCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VDCExtentCommand ERROR] {ex.Message}");
                LowerLeftCorner = new Point2D.Double(0, 0); // Valeur par défaut
                UpperRightCorner = new Point2D.Double(1, 1); // Valeur par défaut
                CgmContext.SetVdcExtent(LowerLeftCorner, UpperRightCorner);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"VDC_EXTENT: LowerLeft=({LowerLeftCorner.X}, {LowerLeftCorner.Y}), " +
                   $"UpperRight=({UpperRightCorner.X}, {UpperRightCorner.Y})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
