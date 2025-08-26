using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class NoOpCommand : BaseCgmCommand
    {
        public NoOpCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
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
            return $"NO_OP";
        }
    }
}
