using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class ConnectingEdgeCommand : BaseCgmCommand
    {
        public ConnectingEdgeCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length) { }

        public override void Draw(Graphics g, Pen pen) { /* Edge connector */ }
        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        public override string ToString() => "CONNECTING_EDGE";
    }
}
