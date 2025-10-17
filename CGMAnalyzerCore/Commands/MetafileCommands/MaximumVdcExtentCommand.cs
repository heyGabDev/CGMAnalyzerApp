using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MaximumVdcExtentCommand : BaseCgmCommand
    {
        public Point2D.Double Point1 { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double Point2 { get; private set; } = new Point2D.Double(0, 0);

        public MaximumVdcExtentCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MaximumVdcExtentCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Point1 = argReader.MakePoint(ec, eid);
                Point2 = argReader.MakePoint(ec, eid);
                Debug.WriteLine($"[MaximumVdcExtentCommand] Point1=({Point1.X}, {Point1.Y}), Point2=({Point2.X}, {Point2.Y})");
                ValidateArgumentsRead("MaximumVdcExtentCommand");   
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaximumVdcExtentCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MAXIMUM_VDC_EXTENT : P1({Point1.X}, {Point1.Y}) P2({Point2.X}, {Point2.Y})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
