using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{    //Commande de structure
    public class BeginPictureCommand : BaseCgmCommand
    {
        public string Name { get; private set; }

        public BeginPictureCommand(int ec, int eid, int l, BinaryReader reader, ExtractedArgumentReader argReader)
            : base(ec, eid, l)
        {
            Name = argReader.MakeString(reader);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"BeginPicture {Name}";
        }
    }
}
