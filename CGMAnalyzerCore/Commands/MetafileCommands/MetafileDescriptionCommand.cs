using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileDescriptionCommand : BaseCgmCommand
    {
        public string Description { get; }

        public MetafileDescriptionCommand(int ec, int eid, int length, ExtractedArgumentReader argReader)
            : base(ec, eid, length)
        {
            Description = argReader.MakeString();
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
