using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class LineTypeCommand : CgmCommand
    {
        public int LineType { get; private set; }

        public LineTypeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            LineType = argReader.MakeIndex();

            ValidateArgumentsRead("LineType");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineType");
        }

        public override string ToString()
        {
            return $"LineType {LineType}";
        }
    }
}
