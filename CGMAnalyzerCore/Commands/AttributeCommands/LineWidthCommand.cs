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
    public class LineWidthCommand : CgmCommand
    {
        public double LineWidth { get; private set; }

        public LineWidthCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            // Utilise makeSizeSpecification selon le mode de spécification
            var mode = CgmContext.LineWidthSpecificationMode;
            LineWidth = mode == SpecificationMode.ABSOLUTE ? argReader.MakeVdc() : argReader.MakeReal();
            ValidateArgumentsRead("LineWidth");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineWidth");
        }

        public override string ToString()
        {
            return $"LineWidth {LineWidth}";
        }
    }
}
