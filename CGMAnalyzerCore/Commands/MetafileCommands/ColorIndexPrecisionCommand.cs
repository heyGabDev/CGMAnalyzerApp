using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    // <summary>
    /// Class=1, Element=8
    /// Colour Index Precision Command
    /// </summary>
    public class ColorIndexPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; }
        private const int DEFAULT_PRECISION = 8;

        public ColorIndexPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorIndexPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                Precision = argReader.MakeInt();
                CgmContext.ColorIndexPrecision = Precision;
                Debug.WriteLine($"[ColorIndexPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("ColorIndexPrecisionCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ColorIndexPrecisionCommand ERROR]  {ex.Message}");
                Precision = DEFAULT_PRECISION;
                CgmContext.ColorIndexPrecision = Precision;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"COLOR_INDEX_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
