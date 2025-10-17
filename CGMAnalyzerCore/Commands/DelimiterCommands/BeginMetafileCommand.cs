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
    public class BeginMetafileCommand : BaseCgmCommand
    {
        public string MetafileName { get; private set; } = "";

        public BeginMetafileCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            var argReader = new ExtractedArgumentReader(this);

            Debug.WriteLine($"[BeginMetafileCommand] ArgsLength={Args?.Length ?? 0}");

            if (Args != null && Args.Length > 0)
            {
                try
                {
                // Lire les arguments avec validation
                MetafileName = argReader.MakeString();
                Debug.WriteLine($"[BeginMetafileCommand] MetafileName='{MetafileName}'");
                ValidateArgumentsRead("BeginMetafileCommand");
                }
                catch (Exception ex)
                {
                        Debug.WriteLine($"[BeginMetafileCommand ERROR]: {ex.Message}");
                        MetafileName = "Error";
                        HasReadErrors = true;
                }
            }
            else
            {
                MetafileName = "Default";
                Debug.WriteLine("[BeginMetafileCommand] Aucun argument, nom par défaut");
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"BEGIN_METAFILE \"{MetafileName}\"";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
