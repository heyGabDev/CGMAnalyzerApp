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
    public class TextColourCommand : CgmCommand
    {
        public System.Drawing.Color TextColor { get; private set; }

        public TextColourCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
            {
                int colorIndex = argReader.MakeColorIndex();
                TextColor = System.Drawing.Color.FromArgb(colorIndex, colorIndex, colorIndex);
            }
            else
            {
                TextColor = argReader.MakeDirectColor();
            }
            ValidateArgumentsRead("TextColor");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in TextColour");
        }

        public override string ToString()
        {
            return $"TextColour {TextColor}";
        }
    }
}
