using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class EdgeJoinCommand : CgmCommand
    {

        public EdgeJoinType EdgeJoin { get; private set; }

        public EdgeJoinCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int join = argReader.MakeIndex();
            EdgeJoin = join switch
            {
                1 => EdgeJoinType.Unspecified,
                2 => EdgeJoinType.Mitre,
                3 => EdgeJoinType.Round,
                4 => EdgeJoinType.Bevel,
                _ => EdgeJoinType.Unspecified
            };

            ValidateArgumentsRead("EdgeJoin");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeJoin");
        }

        public override string ToString()
        {
            return $"EdgeJoin {EdgeJoin}";
        }
    }
}
