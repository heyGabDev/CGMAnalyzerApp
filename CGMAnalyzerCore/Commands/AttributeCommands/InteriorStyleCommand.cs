using CGMAnalyzerCore.Enums.Fonts;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class InteriorStyleCommand : CgmCommand
    {
        public InteriorStyleType Style { get; private set; }

        public InteriorStyleCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int style = argReader.MakeEnum();
            Style = style switch
            {
                0 => InteriorStyleType.Hollow,
                1 => InteriorStyleType.Solid,
                2 => InteriorStyleType.Pattern,
                3 => InteriorStyleType.Hatch,
                4 => InteriorStyleType.Empty,
                5 => InteriorStyleType.GeometricPattern,
                6 => InteriorStyleType.Interpolated,
                _ => InteriorStyleType.Hollow
            };
            ValidateArgumentsRead("InteriorStyle");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in InteriorStyle");
        }

        public override string ToString()
        {
            return $"InteriorStyle {Style}";
        }
    }
}
