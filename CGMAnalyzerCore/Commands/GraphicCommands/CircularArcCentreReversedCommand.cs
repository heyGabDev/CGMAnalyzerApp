using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class CircularArcCentreReversedCommand : BaseCgmCommand
    {
        public CircularArcCentreReversedCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length) { }

        public override void Draw(Graphics g, Pen pen)
        {
            // Même logique que CircularArcCentre mais sens inverse
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => "CIRCULAR_ARC_CENTRE_REVERSED";
    }
}
