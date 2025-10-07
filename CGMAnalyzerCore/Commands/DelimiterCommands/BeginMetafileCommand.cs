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
                if (baseCommand.Args != null && baseCommand.Args.Length > 0)
                {
                    try
                        {
                        // Lire les arguments avec validation
                        MetafileName = argumentReader.ReadString();
                        ValidateArgumentsRead("MetafileName");
                        }
                    catch (Exception ex)
                        {
                            Debug.WriteLine($"[CGM] Erreur lecture BeginMetafile: {ex.Message}");
                            MetafileName = "Error";
                            ErrorCommand = true;
                        }
                }
                else
                {
                    MetafileName = "Default";
                    ErrorCommand = true;
                }
        }

        public override string ToString()
        {
            return $"BEGIN_METAFILE \"{MetafileName}\"";
        }
    }
}
