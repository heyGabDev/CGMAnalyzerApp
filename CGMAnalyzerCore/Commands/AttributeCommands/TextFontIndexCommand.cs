using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class TextFontIndexCommand : CgmCommand
    {
        public int FontIndex { get; private set; }

        public TextFontIndexCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            FontIndex = argReader.MakeIndex();
            ValidateArgumentsRead("TextFontIndex");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in TextFontIndex");
        }

        public override string ToString()
        {
            return $"TextFontIndex {FontIndex}";
        }
    }

}
