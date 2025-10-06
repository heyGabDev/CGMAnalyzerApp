using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class EdgeTypeCommand : CgmCommand
    {
        public int EdgeType { get; private set; }

        public EdgeTypeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            EdgeType = argReader.MakeIndex();
            ValidateArgumentsRead("EdgeType");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeType");
        }

        public override string ToString()
        {
            return $"EdgeType {EdgeType}";
        }
    }
}
