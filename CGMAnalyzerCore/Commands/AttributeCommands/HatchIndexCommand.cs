using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class HatchIndexCommand : CgmCommand
    {
        public int HatchIndex { get; private set; }

        public HatchIndexCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            HatchIndex = argReader.MakeIndex();
            ValidateArgumentsRead("HatchIndex");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in HatchIndex");
        }

        public override string ToString()
        {
            return $"HatchIndex {HatchIndex}";
        }
    }
}
