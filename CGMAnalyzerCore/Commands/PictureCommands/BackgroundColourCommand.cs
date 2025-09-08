using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class BackgroundColorCommand : CgmCommand
    {
        public System.Drawing.Color BackgroundColor { get; private set; }

        public BackgroundColorCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            BackgroundColor = argReader.MakeDirectColor();

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in BackgroundColour");
        }

        public override string ToString()
        {
            return $"BackgroundColour {BackgroundColor}";
        }

        // Méthode paint équivalente du Java (à adapter selon votre architecture de display)
        public void ApplyToDisplay(object display)
        {
            // Dans le Java original : logique complexe de remplissage du background
            // À implémenter selon votre architecture de rendu
        }
    }
}
