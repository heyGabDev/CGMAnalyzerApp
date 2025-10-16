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
    public class RealPrecisionCommand : BaseCgmCommand
    {
        public enum PrecisionType
        {
            FloatingPoint32Bit = 1,
            FloatingPoint64Bit = 2
        }

        public PrecisionType Precision { get; private set; }

        public RealPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[RealPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
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

                CgmContext.RealPrecision = (int)Precision;
                Debug.WriteLine($"[RealPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("RealPrecisionCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RealPrecisionCommand ERROR] {ex.Message}");
                Precision = PrecisionType.FloatingPoint32Bit;// Valeur par défaut en cas d'erreur
                CgmContext.RealPrecision = (int)Precision;
                HasReadErrors = true;
            }

        }

        /// <summary>
        /// Retourne la précision sous forme d'entier pour CgmContext.RealPrecision
        /// </summary>
        public int GetPrecision()
        {
            return (int)Precision;
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"REAL_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");

        }
    }
}
