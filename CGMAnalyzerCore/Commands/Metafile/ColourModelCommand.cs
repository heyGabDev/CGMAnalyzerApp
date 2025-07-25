using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Metafile
{
    public class ColourModelCommand : BaseCgmCommand
    {
        public int ColourModel { get; private set; }

        public ColourModelCommand(int ec, int eid, int l, CgmArgumentReader argReader)
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
