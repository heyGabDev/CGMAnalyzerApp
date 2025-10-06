using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class EndMetafileCommand : CgmCommand
    {
        public EndMetafileCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            // Pas d'arguments selon le Java original

            ValidateArgumentsRead("EndMetafile");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //"Not all arguments were read in EndMetafile");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        //public override void ReadArguments(BinaryReader reader)
        //{
        //    throw new NotImplementedException();
        //}

        public override string ToString()
        {
            return "EndMetafile";
        }
    }
}
