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
    /// TEXT_COLOUR (case 14) - Définit la couleur des textes
    /// </summary>
    public class TextColorCommand : BaseCgmCommand
    {
        public Color TextColor { get; private set; } = Color.Black;
        private Color DEFAULT_TEXT_COLOR = Color.Black;

        public TextColorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
                {
                    int colorIndex = argReader.MakeColorIndex();
                    Debug.WriteLine($"[TextColorCommand] Color index: {colorIndex}");

                    // Récupérer la couleur depuis la table de couleurs
                    TextColor = CgmContext.GetColorFromIndex(colorIndex);
                }
                else
                {
                    TextColor = argReader.MakeDirectColor();
                    Debug.WriteLine($"[TextColorCommand] Direct color: R={TextColor.R}, G={TextColor.G}, B={TextColor.B}");

                }
                ValidateArgumentsRead("TextColorCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextColorCommand Error] {ex.Message}");
                TextColor = DEFAULT_TEXT_COLOR;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"TEXT_COLOR: R={TextColor.R}, G={TextColor.G}, B={TextColor.B}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
