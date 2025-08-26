using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class BeginMetafileCommand : BaseCgmCommand
    {
        public string MetafileName { get; private set; } = "";

        public BeginMetafileCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
            ReadArguments(reader);
        }


        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Les noms CGM sont souvent codés comme une séquence de bytes → ASCII string
            byte[] data = reader.ReadBytes(Length);
            MetafileName = System.Text.Encoding.ASCII.GetString(data);
        }

        public override string ToString()
        {
            return $"BEGIN_METAFILE \"{MetafileName}\"";
        }
    }
}
