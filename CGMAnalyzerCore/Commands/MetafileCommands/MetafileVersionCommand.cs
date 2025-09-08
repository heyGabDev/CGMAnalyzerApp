using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
        /**
     * Class=1, Element=1
     * @author xphc (Philippe Cadé)
     * @author BBNT Solutions
     * @version $Id$
     */
    public class MetafileVersionCommand : CgmCommand
    {
        public int Version { get; private set; }

        public MetafileVersionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
                    : base(baseCommand, ec, eid, l)
        {
            Version = argReader.MakeInt();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Aucune action graphique pour ce type de commande
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"MetafileVersionCommand: Version = {Version}";
        }
    }
}
