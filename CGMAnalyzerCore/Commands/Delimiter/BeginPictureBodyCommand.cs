using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Delimiter
{
    public class BeginPictureBodyCommand : BaseCgmCommand
    {
        public BeginPictureBodyCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
            // Pas d'arguments spécifiques à lire pour BeginPictureBody.
            // Mais on pourrait éventuellement faire reader.ReadBytes(l) si nécessaire.
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Pas d'effet graphique direct, utilisé comme délimiteur logique.
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Begin Picture Body";
        }
    }
}
