using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class TextAlignmentCommand : CgmCommand
    {

        public HorizontalAlignment HAlign { get; private set; }
        public VerticalAlignment VAlign { get; private set; }
        public double ContinuousHorizontalValue { get; private set; }
        public double ContinuousVerticalValue { get; private set; }

        public TextAlignmentCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int hAlign = argReader.MakeEnum();
            int vAlign = argReader.MakeEnum();
            ContinuousHorizontalValue = argReader.MakeReal();
            ContinuousVerticalValue = argReader.MakeReal();

            HAlign = hAlign switch
            {
                0 => HorizontalAlignment.NormalHorizontal,
                1 => HorizontalAlignment.Left,
                2 => HorizontalAlignment.Centre,
                3 => HorizontalAlignment.Right,
                4 => HorizontalAlignment.ContinuousHorizontal,
                _ => HorizontalAlignment.NormalHorizontal
            };

            VAlign = vAlign switch
            {
                0 => VerticalAlignment.NormalVertical,
                1 => VerticalAlignment.Top,
                2 => VerticalAlignment.Cap,
                3 => VerticalAlignment.Half,
                4 => VerticalAlignment.Base,
                5 => VerticalAlignment.Bottom,
                6 => VerticalAlignment.ContinuousVertical,
                _ => VerticalAlignment.NormalVertical
            };

            ValidateArgumentsRead("TextAlignment");

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in TextAlignment");
        }

        public override string ToString()
        {
            return $"TextAlignment h={HAlign} v={VAlign} ch={ContinuousHorizontalValue} cv={ContinuousVerticalValue}";
        }
    }

}
