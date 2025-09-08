using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class NamePrecisionCommand : CgmCommand
    {
        public int Precision { get; private set; }

        public NamePrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Precision = argReader.MakeInt();
            CgmContext.NamePrecision = Precision;

            //System.Diagnostics.Debug.Assert(Precision == 8 || Precision == 16 || Precision == 24 || Precision == 32,
            //    "Invalid name precision");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in NamePrecision");
        }

        public static void Reset()
        {
            CgmContext.NamePrecision = 16;
        }

        public override string ToString()
        {
            return $"NamePrecision {Precision}";
        }
    }
}
