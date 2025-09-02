using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class EndApplicationStructureCommand : CgmCommand
    {
        public EndApplicationStructureCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            // Pas d'arguments selon le Java original
            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in EndApplicationStructure");
        }

        public override string ToString()
        {
            return "EndApplicationStructure";
        }
    }
}
