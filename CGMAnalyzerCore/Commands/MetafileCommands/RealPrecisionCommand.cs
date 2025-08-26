using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class RealPrecisionCommand : BaseCgmCommand
    {
        public enum PrecisionType
        {
            FloatingPoint32Bit = 1,
            FloatingPoint64Bit = 2
        }

        public static PrecisionType Precision { get; private set; }

        public RealPrecisionCommand(int ec, int eid, int l, ExtractedArgumentReader argReader)
            : base(ec, eid, l)
        {
            int p1 = argReader.MakeInt();

            switch (p1)
            {
                case 1:
                    Precision = PrecisionType.FloatingPoint32Bit;
                    break;
                case 2:
                    Precision = PrecisionType.FloatingPoint64Bit;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported REAL precision value: {p1}");
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Pas de rendu graphique requis
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"RealPrecision: {Precision}";
        }
    }
}
