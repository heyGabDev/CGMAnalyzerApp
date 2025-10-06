using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class AlternateCharacterSetIndexCommand : CgmCommand
    {
        public int AlternateCharacterSetIndex { get; private set; }

        public AlternateCharacterSetIndexCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            AlternateCharacterSetIndex = argReader.MakeIndex();
            ValidateArgumentsRead("AlternateCharacterSetIndex");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in AlternateCharacterSetIndex");
        }

        public override string ToString()
        {
            return $"AlternateCharacterSetIndex {AlternateCharacterSetIndex}";
        }
    }
}
