using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Metafile
{
        /**
     * Class=1, Element=1
     * @author xphc (Philippe Cadé)
     * @author BBNT Solutions
     * @version $Id$
     */
    public class MetafileVersionCommand : BaseCgmCommand
    {
        public int Version { get; private set; }

        public MetafileVersionCommand(int ec, int eid, int length, CgmArgumentReader reader)
                    : base(ec, eid, length)
        {
            Version = reader.MakeInt();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Aucune action graphique pour ce type de commande
        }

        public override string ToString()
        {
            return $"MetafileVersionCommand: Version = {Version}";
        }
    }
}
