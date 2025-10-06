using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class BeginApplicationStructureBodyCommand : CgmCommand
    {
        public BeginApplicationStructureBodyCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            // Pas d'arguments selon le Java original

            ValidateArgumentsRead("BeginApplicationStructureBody");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in BeginApplicationStructureBody");
        }

        public override string ToString()
        {
            return "BeginApplicationStructureBody";
        }
    }
}
