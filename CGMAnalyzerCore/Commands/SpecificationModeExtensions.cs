using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.MarkerSizeSpecificationModeCommand;

namespace CGMAnalyzerCore.Commands
{
    public static class SpecificationModeExtensions
    {
        public enum SpecificationMode
        {
            ABSOLUTE = 0,
            SCALED = 1,
            FRACTIONAL = 2,
            MM = 3
        }
        public static SpecificationMode GetMode(int mode)
        {
            return mode switch
            {
                0 => SpecificationMode.ABSOLUTE,
                1 => SpecificationMode.SCALED,
                2 => SpecificationMode.FRACTIONAL,
                3 => SpecificationMode.MM,
                _ => SpecificationMode.ABSOLUTE
            };
        }
    }
}
