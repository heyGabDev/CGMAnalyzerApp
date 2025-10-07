using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class LineWidthSpecificationModeCommand : CgmCommand
    {
        public SpecificationMode Mode { get; private set; }

        public LineWidthSpecificationModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int mode = argReader.MakeEnum();
            Mode = SpecificationModeExtensions.GetMode(mode);

            CgmContext.LineWidthSpecificationMode = Mode;

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineWidthSpecificationMode");
        }

        public static void Reset()
        {
            CgmContext.LineWidthSpecificationMode = SpecificationMode.ABSOLUTE;
        }

        public override string ToString()
        {
            return $"LineWidthSpecificationMode {Mode}";
        }
    }
}
