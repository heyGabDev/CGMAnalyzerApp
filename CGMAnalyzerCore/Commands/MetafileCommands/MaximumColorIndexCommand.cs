using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MaximumColorIndexCommand : CgmCommand
    {
        public int MaxColorIndex { get; private set; }

        public MaximumColorIndexCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            MaxColorIndex = argReader.MakeColorIndex();

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in MaximumColourIndex");
        }

        public override string ToString()
        {
            return $"MaximumColourIndex {MaxColorIndex}";
        }

        // Méthode paint équivalente du Java (à adapter selon votre architecture de display)
        public void ApplyToDisplay(object display)
        {
            // Dans le Java original : d.setMaximumColorIndex(this.maxColorIndex);
            // À implémenter selon votre architecture de rendu
        }
    }
}
