using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.EscapeCommands
{
    public class EscapeCommand : CgmCommand
    {
        public int Identifier { get; private set; }
        public string DataRecord { get; private set; }

        public EscapeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Identifier = argReader.MakeInt();
            DataRecord = argReader.MakeString();

            ValidateArgumentsRead("Escape");
        }

        public override string ToString()
        {
            return $"Escape identifier={Identifier}";
            // Note: DataRecord commenté dans le Java original
            // return $"Escape identifier={Identifier} dataRecord={DataRecord}";
        }
    }
}
