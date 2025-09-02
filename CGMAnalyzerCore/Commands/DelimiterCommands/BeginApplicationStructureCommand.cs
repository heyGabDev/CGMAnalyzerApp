using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class BeginApplicationStructureCommand : CgmCommand
    {
        public string Id { get; private set; }
        public string Type { get; private set; }

        public BeginApplicationStructureCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Id = argReader.MakeString();
            Type = argReader.MakeString();

            // Logic from Java original
            if (Type.Equals("LAYER", StringComparison.OrdinalIgnoreCase))
            {
                LayerId++;
                CgmContext.CurrentLayerId++;
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in BeginApplicationStructure");
        }

        public override string ToString()
        {
            return $"BeginApplicationStructure : {Id} - {Type}";
        }
    }

}
