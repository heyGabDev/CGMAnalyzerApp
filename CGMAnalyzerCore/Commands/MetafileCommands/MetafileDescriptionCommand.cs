using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileDescriptionCommand : BaseCgmCommand
    {
        public string Description { get; } = "";

        public MetafileDescriptionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            // Pas d'arguments selon le Java original
            Args = command.Args;
            Debug.WriteLine($"[MetafileDescriptionCommand] ArgsLength={Args?.Length ?? 0}");

            try 
            { 
                var argReader = new ExtractedArgumentReader(this);
                Description = argReader.MakeString();
                Debug.WriteLine($"[MetafileDescriptionCommand] Description='{Description}'");

                ValidateArgumentsRead("MetafileDescriptionCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MetafileDescriptionCommand ERROR] {ex.Message}");
                Description = "Error"; // Valeur par défaut en cas d'erreur
                HasReadErrors = true;
            }   
        }

        public override void Draw(Graphics g, Pen pen)
        {
            //No drawing
        }

        public override string ToString()
        {
            return $"METAFILE_DESCRIPTION : \"{Description}\"";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
