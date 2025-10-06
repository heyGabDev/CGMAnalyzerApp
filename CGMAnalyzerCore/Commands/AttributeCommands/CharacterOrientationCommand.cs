using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class CharacterOrientationCommand : CgmCommand
    {
        public double XUp { get; private set; }
        public double YUp { get; private set; }
        public double XBase { get; private set; }
        public double YBase { get; private set; }

        public CharacterOrientationCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            XUp = argReader.MakeVdc();
            YUp = argReader.MakeVdc();
            XBase = argReader.MakeVdc();
            YBase = argReader.MakeVdc();
            ValidateArgumentsRead("CharacterOrientation");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in CharacterOrientation");
        }

        public override string ToString()
        {
            return $"CharacterOrientation up=({XUp},{YUp}) base=({XBase},{YBase})";
        }
    }

}
