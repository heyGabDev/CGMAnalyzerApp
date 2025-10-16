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
    public class MarkerColourCommand : CgmCommand
    {
        public System.Drawing.Color MarkerColor { get; private set; }

        public MarkerColourCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
            {
                int colorIndex = argReader.MakeColorIndex();
                MarkerColor = System.Drawing.Color.FromArgb(colorIndex, colorIndex, colorIndex);
            }
            else
            {
                MarkerColor = argReader.MakeDirectColor();
            }

            ValidateArgumentsRead("MarkerColour");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in MarkerColour");
        }

        public override string ToString()
        {
            return $"MarkerColour {MarkerColor}";
        }
    }

}
