using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class ColorModelCommand : BaseCgmCommand
    {
        public int ColourModel { get; private set; }

        public ColorModelCommand(int ec, int eid, int l, ExtractedArgumentReader argReader)
            : base(ec, eid, l)
        {
            ColourModel = argReader.MakeInt();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            throw new NotImplementedException();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"ColourModelCommand: Version = {ColourModel}";
        }
    }
}
