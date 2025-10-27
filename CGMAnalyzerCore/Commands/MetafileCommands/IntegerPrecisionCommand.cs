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
    public class IntegerPrecisionCommand : BaseCgmCommand
    {
        public int IntPrecision { get; }
        private const int DEFAULT_INT_PRECISION = 16;

        public IntegerPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[IntegerPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                IntPrecision = argReader.MakeInt();

                // Mettre à jour le contexte CGM
                CgmContext.IntegerPrecision = IntPrecision;

                Debug.WriteLine($"[IntegerPrecisionCommand] Precision={IntPrecision}");
                ValidateArgumentsRead("IntegerPrecisionCommand");
            }
            catch (Exception)
            {
                IntPrecision = DEFAULT_INT_PRECISION;
                CgmContext.SetIntegerPrecision(IntPrecision);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"INTERGER_PRECISION : {IntPrecision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
