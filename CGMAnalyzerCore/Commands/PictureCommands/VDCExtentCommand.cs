using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class VDCExtentCommand : CgmCommand
    {
        public Point2D.Double LowerLeftCorner { get; private set; }
        public Point2D.Double UpperRightCorner { get; private set; }

        public VDCExtentCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            LowerLeftCorner = argReader.MakePoint();
            UpperRightCorner = argReader.MakePoint();

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in VDCExtent");
        }

        public Point2D.Double[] GetExtent()
        {
            var toto = new Point2D.Double[] { LowerLeftCorner, UpperRightCorner };
            return toto; //new Point2D.Double[] { LowerLeftCorner, UpperRightCorner };
        }

        public override string ToString()
        {
            return $"VDCExtent [{LowerLeftCorner}] [{UpperRightCorner}]";
        }
    }
}
