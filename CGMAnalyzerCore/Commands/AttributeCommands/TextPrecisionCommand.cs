using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class TextPrecisionCommand : CgmCommand
    {

        public TextPrecisionType Precision { get; private set; }

        public TextPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int precision = argReader.MakeEnum();
            Precision = precision switch
            {
                0 => TextPrecisionType.String,
                1 => TextPrecisionType.Character,
                2 => TextPrecisionType.Stroke,
                _ => TextPrecisionType.String
            };

            ValidateArgumentsRead("TextPrecision");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in TextPrecision");
        }

        public override string ToString()
        {
            return $"TextPrecision {Precision}";
        }
    }
}
