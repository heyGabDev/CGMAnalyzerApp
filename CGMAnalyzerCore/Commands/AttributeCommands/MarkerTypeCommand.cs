using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class MarkerTypeCommand : CgmCommand
    {
        public int MarkerType { get; private set; }

        public MarkerTypeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            MarkerType = argReader.MakeIndex();
            ValidateArgumentsRead("MarkerType");

        //    System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
        //        "Not all arguments were read in MarkerType");
        }

        public override string ToString()
        {
            return $"MarkerType {MarkerType}";
        }
    }
}
