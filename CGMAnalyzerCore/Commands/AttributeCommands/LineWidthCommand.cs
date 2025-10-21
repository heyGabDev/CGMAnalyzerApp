using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// LINE_WIDTH (case 3) - Définit la largeur de ligne
    /// </summary>
    public class LineWidthCommand : BaseCgmCommand
    {
        public double LineWidth { get; private set; } = 1.0;
        private const double DEFAULT_LINE_WIDTH = 1.0;

        public LineWidthCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineWidthCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Utilise makeSizeSpecification selon le mode de spécification
                var mode = CgmContext.LineWidthSpecificationMode;
                if (mode == SpecificationMode.ABSOLUTE)
                {
                    LineWidth = argReader.MakeVdc();
                    Debug.WriteLine($"[LineWidthCommand] LineWidth (absolute): {LineWidth:F2}");
                }
                else // SCALED
                {
                    LineWidth = argReader.MakeReal();
                    Debug.WriteLine($"[LineWidthCommand] LineWidth (scaled): {LineWidth:F4}");
                }
                ValidateArgumentsRead("LineWidth");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($" [LineWidthCommand Error] {ex.Message}");
                LineWidth = DEFAULT_LINE_WIDTH;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_WIDTH : {LineWidth}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
