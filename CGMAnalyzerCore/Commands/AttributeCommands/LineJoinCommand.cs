using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class LineJoinCommand : CgmCommand
    {

        public LineJoinType LineJoin { get; private set; }

        public LineJoinCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int join = argReader.MakeIndex();
            LineJoin = join switch
            {
                1 => LineJoinType.Unspecified,
                2 => LineJoinType.Mitre,
                3 => LineJoinType.Round,
                4 => LineJoinType.Bevel,
                _ => LineJoinType.Unspecified
            };

            ValidateArgumentsRead("LineJoin");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineJoin");
        }

        public override string ToString()
        {
            return $"LineJoin {LineJoin}";
        }
    }
}
