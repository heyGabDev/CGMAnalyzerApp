using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class RealPrecisionCommand : CgmCommand
    {
        public enum PrecisionType
        {
            FloatingPoint32Bit = 1,
            FloatingPoint64Bit = 2
        }

        public static PrecisionType Precision { get; private set; }

        public RealPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
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

        /// <summary>
        /// Retourne la précision sous forme d'entier pour CgmContext.RealPrecision
        /// </summary>
        public int GetPrecision()
        {
            return (int)Precision;
        }

        /// <summary>
        /// Retourne la précision statique courante
        /// </summary>
        public static int GetCurrentPrecision()
        {
            return (int)Precision;
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
