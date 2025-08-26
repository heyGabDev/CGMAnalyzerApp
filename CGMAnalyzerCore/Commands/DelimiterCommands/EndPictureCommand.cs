using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class EndPictureCommand : BaseCgmCommand
    {
        public EndPictureCommand(int ec, int eid, int l, ExtractedArgumentReader argReader)
            : base(ec, eid, l)
        {
            // Rien à lire ici, la commande ne contient pas d’arguments
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"END_PICTURE";
        }
    }
}
