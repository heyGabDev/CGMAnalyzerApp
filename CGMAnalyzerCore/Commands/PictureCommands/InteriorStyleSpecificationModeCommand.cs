using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class InteriorStyleSpecificationModeCommand : CgmCommand
    {
        public SpecificationMode Mode { get; private set; }

        public InteriorStyleSpecificationModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int mod = argReader.MakeEnum();
            Mode = mod switch
            {
                0 => SpecificationMode.ABSOLUTE,
                1 => SpecificationMode.SCALED,
                2 => SpecificationMode.FRACTIONAL,
                3 => SpecificationMode.MM,
                _ => SpecificationMode.ABSOLUTE
            };

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in InteriorStyleSpecificationMode");
        }

        public override string ToString()
        {
            return $"InteriorStyleSpecificationMode mode={Mode}";
        }
    }
}
