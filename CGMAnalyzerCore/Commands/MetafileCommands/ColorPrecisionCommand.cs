using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class ColorPrecisionCommand : CgmCommand
    {
        public int Precision { get; private set; }

        public ColorPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Precision = argReader.MakeInt();
            CgmContext.ColorPrecision = Precision;

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ColourPrecision");
        }

        public static void Reset()
        {
            CgmContext.ColorPrecision = 8;
        }

        public override string ToString()
        {
            return $"ColourPrecision {Precision}";
        }
    }
}
