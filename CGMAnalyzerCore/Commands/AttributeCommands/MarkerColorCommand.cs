using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.ColorSelectionModeCommand;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// MARKER_COLOUR (case 8) - Définit la couleur des marqueurs
    /// </summary>
    public class MarkerColorCommand : BaseCgmCommand
    {
        public Color MarkerColor { get; private set; } = Color.Black;
        private readonly Color DEFAULT_MARKER_COLOR = Color.Black;

        public MarkerColorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MarkerColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
                {
                    int makerColorIndex = argReader.MakeColorIndex();
                    Debug.WriteLine($"[MarkerColorCommand] Color index: {MarkerColor}");

                    // Récupérer la couleur depuis la table de couleurs
                    MarkerColor = CgmContext.GetColorFromIndex(makerColorIndex);
                }
                else
                {
                    MarkerColor = argReader.MakeDirectColor();
                }
                Debug.WriteLine($"[MarkerColorCommand] {MarkerColor}");
                ValidateArgumentsRead("MarkerColorCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MarkerColorCommand Error] {ex.Message}");
                MarkerColor = DEFAULT_MARKER_COLOR;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MARKER_COLOR : R={MarkerColor.R}, G={MarkerColor.G}, B={MarkerColor.B}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
