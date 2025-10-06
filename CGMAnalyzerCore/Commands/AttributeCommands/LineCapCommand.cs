using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class LineCapCommand : CgmCommand
    {

        public LineCapType LineCap { get; private set; }

        public LineCapCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int cap = argReader.MakeIndex();
            LineCap = cap switch
            {
                1 => LineCapType.Unspecified,
                2 => LineCapType.Butt,
                3 => LineCapType.Round,
                4 => LineCapType.ProjectingSquare,
                5 => LineCapType.Triangle,
                _ => LineCapType.Unspecified
            };

            ValidateArgumentsRead("LineCap");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineCap");
        }

        public override string ToString()
        {
            return $"LineCap {LineCap}";
        }
    }
}
