using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class LineAndEdgeTypeDefinitionCommand : BaseCgmCommand
    {
        public int LineType { get; private set; }
        public List<double> DashPattern { get; private set; } = new List<double>();
        private const int DEFAULT_LINE_TYPE = 1;

        public LineAndEdgeTypeDefinitionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineAndEdgeTypeDefinitionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                LineType = argReader.MakeIndex();
                DashPattern = new List<double>();

                // Lire le pattern de tirets (simplifié)
                if (Args != null)
                {
                    while (CurrentArg < Args.Length)
                    {
                        DashPattern.Add(argReader.MakeReal());
                    }
                }
                Debug.WriteLine($"[LineAndEdgeTypeDefinitionCommand] LineType={LineType}, DashPattern=[{string.Join(",", DashPattern)}]");
                ValidateArgumentsRead("LineAndEdgeTypeDefinitionCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineAndEdgeTypeDefinitionCommand ERROR] {ex.Message}");
                LineType = DEFAULT_LINE_TYPE;
                DashPattern = new List<double>();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_AND_EDGE_TYPE_DEFINITION : type={LineType} pattern=[{string.Join(",", DashPattern)}]";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
