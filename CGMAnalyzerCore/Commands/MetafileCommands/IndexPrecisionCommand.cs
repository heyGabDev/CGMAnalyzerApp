using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class IndexPrecisionCommand : CgmCommand
    {
        public int Precision { get; private set; }

        public IndexPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Precision = argReader.MakeInt();
            CgmContext.IndexPrecision = Precision;

            //System.Diagnostics.Debug.Assert(Precision == 8 || Precision == 16 || Precision == 24 || Precision == 32,
            //    "Invalid index precision");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in IndexPrecision");
        }

        public static void Reset()
        {
            CgmContext.IndexPrecision = 16;
        }

        public override string ToString()
        {
            return $"IndexPrecision {Precision}";
        }
    }
}
