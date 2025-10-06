using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class ColourTableCommand : CgmCommand
    {
        public int StartIndex { get; private set; }
        public List<System.Drawing.Color> Colors { get; private set; }

        public ColourTableCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            StartIndex = argReader.MakeColorIndex();
            Colors = new List<System.Drawing.Color>();

            // Lire les couleurs jusqu'à épuisement des arguments
            while (CurrentArg < Args.Length)
            {
                Colors.Add(argReader.MakeDirectColor());
            }
            ValidateArgumentsRead("ColourTable");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in ColourTable");
        }

        public override string ToString()
        {
            return $"ColourTable startIndex={StartIndex} colors={Colors.Count}";
        }
    }
}
