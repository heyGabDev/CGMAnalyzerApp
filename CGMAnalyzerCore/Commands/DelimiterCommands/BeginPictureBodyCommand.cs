using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class BeginPictureBodyCommand : CgmCommand
    {
        public BeginPictureBodyCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //"Not all arguments were read in BeginPictureBody");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Pas d'effet graphique direct, utilisé comme délimiteur logique.
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "BEGIN_PICTURE_BODY";
        }
    }
}
