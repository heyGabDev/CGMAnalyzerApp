using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class CharacterSetIndexCommand : CgmCommand
    {
        public int CharacterSetIndex { get; private set; }

        public CharacterSetIndexCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            CharacterSetIndex = argReader.MakeIndex();
            ValidateArgumentsRead("CharacterSetIndex");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in CharacterSetIndex");
        }

        public override string ToString()
        {
            return $"CharacterSetIndex {CharacterSetIndex}";
        }
    }
}
