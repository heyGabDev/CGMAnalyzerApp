using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class EdgeVisibilityCommand : CgmCommand
    {
        public bool EdgeVisible { get; private set; }

        public EdgeVisibilityCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            EdgeVisible = argReader.MakeEnum() == 1;
            ValidateArgumentsRead("EdgeVisibility");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeVisibility");
        }

        public override string ToString()
        {
            return $"EdgeVisibility {EdgeVisible}";
        }
    }
}
