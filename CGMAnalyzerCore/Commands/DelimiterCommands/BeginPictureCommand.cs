using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{    //Commande de structure
    public class BeginPictureCommand : BaseCgmCommand
    {

        public BeginPictureCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            // Pas d'arguments selon le Java original
            Args = command.Args;
            ValidateArgumentsRead("BeginPicture");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"BEGIN_PICTURE";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
