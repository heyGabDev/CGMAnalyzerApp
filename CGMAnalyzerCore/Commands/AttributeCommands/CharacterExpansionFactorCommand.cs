using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class CharacterExpansionFactorCommand : CgmCommand
    {
        public double ExpansionFactor { get; private set; }

        public CharacterExpansionFactorCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            ExpansionFactor = argReader.MakeReal();
            ValidateArgumentsRead("ExpansionFactor");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in CharacterExpansionFactor");
        }

        public override string ToString()
        {
            return $"CharacterExpansionFactor {ExpansionFactor}";
        }
    }
}
