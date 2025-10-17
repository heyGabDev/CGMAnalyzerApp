using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class BeginApplicationStructureCommand : BaseCgmCommand
    {
       // public string BeginApplicationStructure { get; private set; } = "";
        public string Id { get; private set; }
        public string Type { get; private set; }

        public BeginApplicationStructureCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[BeginApplicationStructure] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Debug.WriteLine("[BeginApplicationStructure] Lecture des arguments...");
                if (Args != null && Args.Length > 0)
                {
                    // Lire les deux chaînes dans l'ordre
                    Id = argReader.MakeString();
                    Type = argReader.MakeString();
                    Debug.WriteLine($"[BeginApplicationStructure] Id='{Id}' Type='{Type}'");

                    // Logic from Java original
                    if (Type.Equals("LAYER", StringComparison.OrdinalIgnoreCase))
                    {
                        CgmContext.CurrentLayerId++;
                        Debug.WriteLine($"[BeginApplicationStructure] Layer ID incrémenté: {CgmContext.CurrentLayerId}");
                    }

                    ValidateArgumentsRead("BeginApplicationStructure");
                }
                else
                {
                    Id = "Unknown";
                    Type = "Default";
                    Debug.WriteLine("[BeginApplicationStructure] Aucun argument, valeurs par défaut");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeginApplicationStructure ERROR] {ex.Message}");
                Id = "Error";
                Type = "Error";
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"BEGIN_APPLICATION_STRUCTURE : ID ={Id} - TYPE ={Type}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
