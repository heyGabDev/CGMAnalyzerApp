using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Delimiter
{
    public class EndMetafileCommand : BaseCgmCommand
    {
        public EndMetafileCommand(int ec, int eid, int l, BinaryReader reader)
            : base(ec, eid, l)
        {
            // No additional parsing needed for this command
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
            return "EndMetafile";
        }
    }
}
