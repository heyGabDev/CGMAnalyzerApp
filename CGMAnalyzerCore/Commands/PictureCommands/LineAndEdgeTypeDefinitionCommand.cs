using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class LineAndEdgeTypeDefinitionCommand : CgmCommand
    {
        public int LineType { get; private set; }
        public List<double> DashPattern { get; private set; }

        public LineAndEdgeTypeDefinitionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            LineType = argReader.MakeIndex();
            DashPattern = new List<double>();

            // Lire le pattern de tirets (simplifié)
            while (CurrentArg < Args.Length)
            {
                DashPattern.Add(argReader.MakeReal());
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in LineAndEdgeTypeDefinition");
        }

        public override string ToString()
        {
            return $"LineAndEdgeTypeDefinition type={LineType} pattern=[{string.Join(",", DashPattern)}]";
        }
    }
}
