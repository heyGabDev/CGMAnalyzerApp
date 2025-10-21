using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class GeneralizedDrawingPrimitiveCommand : BaseCgmCommand
    {
        public GeneralizedDrawingPrimitiveCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l) 
        {
            Args = command.Args;
            Debug.WriteLine($"[GeneralizedDrawingPrimitiveCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

               // Consommation explicite
               // Consommer tous les arguments sans les interpréter
               int remaining = this.RemainingArgs();
               for (int i = 0; i < remaining; i++)
               {
                   argReader.MakeUInt8(); // Jeter les données
               }

                Debug.WriteLine("[GeneralizedDrawingPrimitiveCommand] Commande non supportée - arguments ignorés");
                ValidateArgumentsRead("GeneralizedDrawingPrimitiveCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GeneralizedDrawingPrimitiveCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"GENERALIZED_DRAWING_PRIMITIVE (unsupported)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
