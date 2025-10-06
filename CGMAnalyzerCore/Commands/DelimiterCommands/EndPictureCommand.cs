using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class EndPictureCommand : CgmCommand
    {
        public EndPictureCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            // Pas d'arguments selon le Java original

            ValidateArgumentsRead("EndPicture");
            //  System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //"Not all arguments were read in EndPicture");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        // TO DELETE : Géré par CgmCommand
        //public override void ReadArguments(BinaryReader reader)
        //{
        //    throw new NotImplementedException();
        //}

        public override string ToString()
        {
            return $"END_PICTURE";
        }
    }
}
