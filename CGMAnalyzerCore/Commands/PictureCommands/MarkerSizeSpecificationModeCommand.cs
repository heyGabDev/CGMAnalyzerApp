using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Converter.Enums;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class MarkerSizeSpecificationModeCommand : CgmCommand
    {
        public SpecificationMode Mode { get; private set; }

        public MarkerSizeSpecificationModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int mode = argReader.MakeEnum();
            Mode = SpecificationModeExtensions.GetMode(mode);

            CgmContext.MarkerSizeSpecificationMode = Mode;

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in MarkerSizeSpecificationMode");
        }

        public static void Reset()
        {
            CgmContext.MarkerSizeSpecificationMode = SpecificationMode.ABSOLUTE;
        }

        public override string ToString()
        {
            return $"MarkerSizeSpecificationMode {Mode}";
        }
    }
}
