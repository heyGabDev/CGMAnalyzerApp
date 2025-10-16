using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.ColorSelectionModeCommand;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class LineColourCommand : CgmCommand
    {
        public System.Drawing.Color LineColor { get; private set; }

        public LineColourCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            // Selon le mode de sélection des couleurs
            if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
            {
                int colorIndex = argReader.MakeColorIndex();
                // Convertir l'index en couleur (à implémenter selon votre logique)
                LineColor = System.Drawing.Color.FromArgb(colorIndex, colorIndex, colorIndex);
            }
            else
            {
                LineColor = argReader.MakeDirectColor();
            }

            ValidateArgumentsRead("LineColor");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in LineColour");
        }

        public override string ToString()
        {
            return $"LineColour {LineColor}";
        }
    }
}
