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
    public class FillColourCommand : CgmCommand
    {
        public System.Drawing.Color FillColor { get; private set; }

        public FillColourCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
            {
                int colorIndex = argReader.MakeColorIndex();
                FillColor = System.Drawing.Color.FromArgb(colorIndex, colorIndex, colorIndex);
            }
            else
            {
                FillColor = argReader.MakeDirectColor();
            }

            ValidateArgumentsRead("FillColour");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in FillColour");
        }

        public override string ToString()
        {
            return $"FillColour {FillColor}";
        }
    }
}
