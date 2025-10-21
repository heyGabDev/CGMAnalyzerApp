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
    /// LINE_COLOUR (case 4) - Définit la couleur de ligne
    /// </summary>
    public class LineColorCommand : BaseCgmCommand
    {
        public Color LineColor { get; private set; } = Color.Black;
        private readonly Color DEFAULT_LINE_COLOR = Color.Black;
        public LineColorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Selon le mode de sélection des couleurs
                if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
                {
                    int colorIndex = argReader.MakeColorIndex();
                    Debug.WriteLine($"[LineColorCommand] Color index: {colorIndex}");

                    // Récupérer la couleur depuis la table de couleurs
                    LineColor = CgmContext.GetColorFromIndex(colorIndex);
                }
                else
                {
                    LineColor = argReader.MakeDirectColor();
                }

                Debug.WriteLine($"[LineColorCommand] Direct color: R={LineColor.R}, G={LineColor.G}, B={LineColor.B}");
                ValidateArgumentsRead("LineColor");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineColorCommand Error] {ex.Message}");
                LineColor = DEFAULT_LINE_COLOR;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_COLOR: R={LineColor.R}, G={LineColor.G}, B={LineColor.B}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
