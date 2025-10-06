using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class TextPathCommand : CgmCommand
    {

        public TextPathType Path { get; private set; }

        public TextPathCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int path = argReader.MakeEnum();
            Path = path switch
            {
                0 => TextPathType.Right,
                1 => TextPathType.Left,
                2 => TextPathType.Up,
                3 => TextPathType.Down,
                _ => TextPathType.Right
            };

            ValidateArgumentsRead("TextPath");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in TextPath");
        }

        public override string ToString()
        {
            return $"TextPath {Path}";
        }
    }
}
