using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.ColourSelectionModeCommand;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class EdgeColourCommand : CgmCommand
    {
        public System.Drawing.Color EdgeColor { get; private set; }

        public EdgeColourCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
            {
                int colorIndex = argReader.MakeColorIndex();
                EdgeColor = System.Drawing.Color.FromArgb(colorIndex, colorIndex, colorIndex);
            }
            else
            {
                EdgeColor = argReader.MakeDirectColor();
            }

            ValidateArgumentsRead("EdgeColour");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in EdgeColour");
        }

        public override string ToString()
        {
            return $"EdgeColour {EdgeColor}";
        }
    }
}
