using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class MarkerSizeCommand : CgmCommand
    {
        public double MarkerSize { get; private set; }

        public MarkerSizeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            var mode = CgmContext.MarkerSizeSpecificationMode;
            MarkerSize = mode == SpecificationMode.ABSOLUTE ? argReader.MakeVdc() : argReader.MakeReal();

            ValidateArgumentsRead("MarkerSize");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in MarkerSize");
        }

        public override string ToString()
        {
            return $"MarkerSize {MarkerSize}";
        }
    }
}
