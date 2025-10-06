using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class EdgeCapCommand : CgmCommand
    {

        public EdgeCapType EdgeCap { get; private set; }

        public EdgeCapCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int cap = argReader.MakeIndex();
            EdgeCap = cap switch
            {
                1 => EdgeCapType.Unspecified,
                2 => EdgeCapType.Butt,
                3 => EdgeCapType.Round,
                4 => EdgeCapType.ProjectingSquare,
                5 => EdgeCapType.Triangle,
                _ => EdgeCapType.Unspecified
            };

            ValidateArgumentsRead("EdgeCap");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeCap");
        }

        public override string ToString()
        {
            return $"EdgeCap {EdgeCap}";
        }
    }

}
