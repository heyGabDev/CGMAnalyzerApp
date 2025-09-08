using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ControlCommands
{
    public class ClipIndicatorCommand : CgmCommand
    {
        public bool ClipFlag { get; private set; }

        public ClipIndicatorCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            ClipFlag = argReader.MakeEnum() == 1;

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ClipIndicator");
        }

        public override string ToString()
        {
            return $"ClipIndicator {ClipFlag}";
        }

        // Méthode paint équivalente du Java (à adapter selon votre architecture de display)
        public void ApplyToDisplay(object display)
        {
            // Dans le Java original : 
            // d.setClipFlag(this.flag);
            // if (this.flag == false) { g2d.setClip(null); }
            // À implémenter selon votre architecture de rendu
        }
    }

}
