using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class CharacterSpacingCommand : CgmCommand
    {
        public double Spacing { get; private set; }

        public CharacterSpacingCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Spacing = argReader.MakeReal();
            ValidateArgumentsRead("CharacterSpacing");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in CharacterSpacing");
        }

        public override string ToString()
        {
            return $"CharacterSpacing {Spacing}";
        }
    }
}
