using CGMAnalyzerCore.Context;
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
    public class MarkerSizeCommand : BaseCgmCommand
    {
        /// <summary>
        /// MARKER_SIZE (case 7) - Définit la taille de marqueur
        /// </summary>
        public double MarkerSize { get; private set; } = 1.0;
        private const double DEFAULT_MARKER_SIZE = 1.0;

        public MarkerSizeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MarkerSizeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Utilise makeSizeSpecification selon le mode de spécification
                var mode = CgmContext.MarkerSizeSpecificationMode;
                if (mode == SpecificationMode.ABSOLUTE)
                {
                    MarkerSize = argReader.MakeVdc();
                }
                else
                {
                    MarkerSize = argReader.MakeReal();
                }
                Debug.WriteLine($"[MarkerSizeCommand] MarkerSize (scaled): {MarkerSize}");
                ValidateArgumentsRead("MarkerSizeCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MarkerSize = DEFAULT_MARKER_SIZE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MAKER_SIZE : {MarkerSize : F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
