using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Metafile
{
    public class MetafileDescriptionCommand : BaseCgmCommand
    {
        public string Description { get; }

        public MetafileDescriptionCommand(int ec, int eid, int length, CgmArgumentReader argReader)
            : base(ec, eid, length)
        {
            Description = argReader.MakeString(length);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Pas de dessin nécessaire
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"MetafileDescription: \"{Description}\"";
        }
    }
}
