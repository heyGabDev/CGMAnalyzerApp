using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.ColorSelectionModeCommand;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// EDGE_COLOR (case 29) - Définit 
    /// </summary>
    public class EdgeColorCommand : BaseCgmCommand
    {
        public Color EdgeColor { get; private set; } = Color.Black;
        private readonly Color DEFAULT_EDGE_COLOR = Color.Black;

        public EdgeColorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
                {
                    int colorIndex = argReader.MakeColorIndex();
                    Debug.WriteLine($"[EdgeColorCommand] Color index: {colorIndex}");

                    // Récupérer la couleur depuis la table de couleurs
                    EdgeColor = CgmContext.GetColorFromIndex(colorIndex);
                }
                else
                {
                    EdgeColor = argReader.MakeDirectColor();
                }
                Debug.WriteLine($"[EdgeColorCommand] Edge color: R={EdgeColor.R}, G={EdgeColor.G}, B={EdgeColor.B}");
                ValidateArgumentsRead("EdgeColorCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeColorCommand Error] {ex.Message}");
                EdgeColor = DEFAULT_EDGE_COLOR;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_COLOR : {EdgeColor} R={EdgeColor.R}, G={EdgeColor.G}, B={EdgeColor.B}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
