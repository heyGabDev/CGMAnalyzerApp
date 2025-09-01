using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class GeneralizedDrawingPrimitiveCommand : BaseCgmCommand
    {
        public GeneralizedDrawingPrimitiveCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length) { }

        public override void Draw(Graphics g, Pen pen)
        {
            // GDP - commande personnalisée, implémentation dépend du contexte
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => "GENERALIZED_DRAWING_PRIMITIVE";
    }
}
