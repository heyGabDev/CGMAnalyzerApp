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
            FixedPoint32Bit = 0,
            FixedPoint64Bit = 1,
            FloatingPoint32Bit = 2,
            FloatingPoint64Bit = 3  
        }

        public PrecisionType Precision { get; private set; }
        public int FieldWidth { get; private set; }
        public int FractionWidth { get; private set; }
        private const PrecisionType DEFAULT_PRECISION = PrecisionType.FloatingPoint32Bit;
        public RealPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[RealPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int format = argReader.MakeEnum();    // 2 bytes
                FieldWidth = argReader.MakeInt();     // 2 bytes
                FractionWidth = argReader.MakeInt();  // 2 bytes

                Debug.WriteLine($"[RealPrecisionCommand] Format={format}, FieldWidth={FieldWidth}, FractionWidth={FractionWidth}");

                switch (format)
                {
                    case 0: // FixedPoint 32-bit
                        Precision = PrecisionType.FloatingPoint32Bit;
                        CgmContext.RealPrecision = (int)Precision;
                        Debug.WriteLine("[RealPrecisionCommand] Format 0: Using 32-bit floating point");
                        break;

                    case 1: // FixedPoint 64-bit
                        Precision = PrecisionType.FloatingPoint64Bit;
                        CgmContext.RealPrecision = (int)Precision;
                        Debug.WriteLine($"[RealPrecisionCommand] Format 1: Using 64-bit floating point");
                        break;

                    case 2: // FloatingPoint 32-bit
                        Precision = PrecisionType.FloatingPoint64Bit;
                        CgmContext.RealPrecision = (int)Precision;
                        Debug.WriteLine($"[RealPrecisionCommand] Format 3: Using 64-bit fixed point");
                        break;

                    case 3:  // FloatingPoint 64-bit
                        Precision = PrecisionType.FloatingPoint64Bit;
                        CgmContext.RealPrecision = (int)Precision;
                        Debug.WriteLine($"[RealPrecisionCommand] Format 3: Using 64-bit fixed point");
                        break;

                    default:
                        Debug.WriteLine($"[RealPrecisionCommand WARNING] Unknown format {format}, using 32-bit default");
                        Precision = PrecisionType.FloatingPoint32Bit;
                        CgmContext.RealPrecision = (int)Precision;
                        HasReadErrors = true;  // Marquer l'erreur sans planter
                        break;
                }

                ValidateArgumentsRead("RealPrecisionCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RealPrecisionCommand ERROR] {ex.Message}");
                Precision = DEFAULT_PRECISION;
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
