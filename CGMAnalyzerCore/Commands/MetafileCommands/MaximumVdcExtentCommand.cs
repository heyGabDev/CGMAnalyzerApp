using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MaximumVdcExtentCommand : BaseCgmCommand
    {
        public Point2D.Double Point1 { get; }
        public Point2D.Double Point2 { get; }

        public MaximumVdcExtentCommand(int ec, int eid, int l, ExtractedArgumentReader argReader)
            : base(ec, eid, l)
        {
            Point1 = argReader.MakePoint(ec, eid);
            Point2 = argReader.MakePoint(ec, eid);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            throw new NotImplementedException();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"MaximumVdcExtentCommand: P1({Point1.X}, {Point1.Y}) P2({Point2.X}, {Point2.Y})";
        }
    }
}
