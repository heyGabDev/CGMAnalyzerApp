using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class EdgeWidthCommand : CgmCommand
    {
        public double EdgeWidth { get; private set; }

        public EdgeWidthCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            var mode = CgmContext.EdgeWidthSpecificationMode;
            EdgeWidth = mode == SpecificationMode.ABSOLUTE ? argReader.MakeVdc() : argReader.MakeReal();
            ValidateArgumentsRead("EdgeWidth");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeWidth");
        }

        public override string ToString()
        {
            return $"EdgeWidth {EdgeWidth}";
        }
    }
}
