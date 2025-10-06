using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ExternalCommands
{
    public class ApplicationDataCommand : CgmCommand
    {
        public int Identifier { get; private set; }
        public string Data { get; private set; }

        public ApplicationDataCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Identifier = argReader.MakeInt();
            Data = argReader.MakeString();

            ValidateArgumentsRead("ApplicationData");
        }

        public int GetIdentifier()
        {
            return Identifier;
        }

        public string GetData()
        {
            return Data;
        }

        public override string ToString()
        {
            return $"ApplicationData: {Identifier} {Data}";
        }
    }
}
