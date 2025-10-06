using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class CharacterHeightCommand : CgmCommand
    {
        public double Height { get; private set; }

        public CharacterHeightCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Height = argReader.MakeVdc();
            ValidateArgumentsRead("Height");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in CharacterHeight");
        }

        public override string ToString()
        {
            return $"CharacterHeight {Height}";
        }
    }
}
