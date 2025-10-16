using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var argReader = new ExtractedArgumentReader(command);
                LowerLeftCorner = argReader.MakePoint();
                UpperRightCorner = argReader.MakePoint();

                CgmContext.SetVdcExtent(LowerLeftCorner, UpperRightCorner);
                Debug.WriteLine($"[VDCExtentCommand] LowerLeftCorner={LowerLeftCorner}, " +
                                $"UpperRightCorner={UpperRightCorner}");
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
