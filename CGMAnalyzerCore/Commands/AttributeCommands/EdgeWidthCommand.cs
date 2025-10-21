using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// EDGE_WIDTH (case 28) - Définit 
    /// </summary>
    public class EdgeWidthCommand : BaseCgmCommand
    {
        public double EdgeWidth { get; private set; } = 1.0;
        private const double DEFAULT_EDGE_WIDTH = 1.0;
        public EdgeWidthCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeWidthCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                var mode = CgmContext.EdgeWidthSpecificationMode;
                EdgeWidth = mode == SpecificationMode.ABSOLUTE ? argReader.MakeVdc() : argReader.MakeReal();
                if (mode == SpecificationMode.ABSOLUTE)
                {
                    argReader.MakeVdc();
                }
                else
                {
                    argReader.MakeReal();
                }
                Debug.WriteLine($"[EdgeWidthCommand] EdgeWidth : mode={mode}, edgeWith={EdgeWidth}");

                ValidateArgumentsRead("EdgeWidthCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeWidthCommand Error] {ex.Message}");
                EdgeWidth = DEFAULT_EDGE_WIDTH;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_WIDTH : {EdgeWidth}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
