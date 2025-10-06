using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class EndTileArrayCommand : CgmCommand
    {
        public EndTileArrayCommand(int ec, int eid, int l, CgmCommand baseCommand)
            : base(baseCommand, ec, eid, l)
        {
            // Pas d'arguments selon le Java original

            ValidateArgumentsRead("EndTileArray");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EndTileArray");
        }

        public override string ToString()
        {
            return "EndTileArray";
        }

        // Méthode paint équivalente du Java
        public void SetTileArrayInfo(object displayContext)
        {
            // Dans le Java original : d.setTileArrayInfo(null);
            // À implémenter selon votre architecture de rendu
        }
    }
}
