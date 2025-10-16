using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileVersionCommand : BaseCgmCommand
    {
        public int Version { get; private set; }

        public MetafileVersionCommand(int ec, int eid, int l, CgmCommand command)
                    : base(ec, eid, l)
        {
            // Pas d'arguments selon le Java original
            Args = command.Args;
            Debug.WriteLine($"[MetafileVersionCommand] ArgsLength={Args?.Length ?? 0}");

            try 
            {
                var argReader = new ExtractedArgumentReader(command);
                Version = argReader.MakeInt();
                Debug.WriteLine($"[MetafileVersionCommand] Version={Version}");
                ValidateArgumentsRead("MetafileVersionCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MetafileVersionCommand ERROR] {ex.Message}");
                Version = 1; // Version par défaut
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"METAFILE_VERSION : {Version}";
        }
        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
