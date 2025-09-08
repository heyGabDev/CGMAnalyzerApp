using CGMAnalyzerCore.Converter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands
{
    public static class SpecificationModeExtensions
    {
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
