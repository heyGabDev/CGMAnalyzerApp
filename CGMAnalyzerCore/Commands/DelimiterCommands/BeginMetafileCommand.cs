using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    //Commande de structure
    public class BeginMetafileCommand : CgmCommand
    {
        public string MetafileName { get; private set; } = "";

        public BeginMetafileCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argumentReader)
            : base(baseCommand,ec, eid, l)
        {
            try
            {
                if (baseCommand.Args != null && baseCommand.Args.Length > 0)
                {
                    // Lire les arguments avec validation
                    MetafileName = argumentReader.ReadString();
                }
                else
                {
                    MetafileName = "Default";
                    ErrorCommand = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CGM] Erreur lecture BeginMetafile: {ex.Message}");
                MetafileName = "Error";
                ErrorCommand = true;
            }

            ValidateArgumentsRead("MetafileName");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in MetafileName");
        }


        // TO DELETE : Géré par CgmCommand
        //public override void ReadArguments(BinaryReader reader)
        //{
        //    // Les noms CGM sont souvent codés comme une séquence de bytes → ASCII string
        //    byte[] data = reader.ReadBytes(Length);
        //    MetafileName = System.Text.Encoding.ASCII.GetString(data);
        //}

        public override string ToString()
        {
            return $"BEGIN_METAFILE \"{MetafileName}\"";
        }
    }
}
