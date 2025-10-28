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
        /// <summary>
        /// Selon la spécification CGM ISO 8632
        /// </summary>
        /// Format CGM  |       Type            |     Valeur enum     
        ///     0       |    Floating 32-bit    |     FloatingPoint32Bit (2)
        ///     1       |    Floating 64-bit    |     FloatingPoint64Bit (3)  
        ///     2       |    Fixed 32-bit       |    FixedPoint32Bit (0)
        ///     3       |    Fixed 64-bit       |    FixedPoint64Bit (1)
        
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
                    case 0: // CGM Format 0 = Floating 32-bit
                        Precision = PrecisionType.FloatingPoint32Bit;  // = 2
                        CgmContext.RealPrecision = (int)Precision;     // = 2
                        Debug.WriteLine("[RealPrecisionCommand] Format 0: FloatingPoint32");
                        break;

                    case 1: // CGM Format 1 = Floating 64-bit
                        Precision = PrecisionType.FloatingPoint64Bit;  // = 3
                        CgmContext.RealPrecision = (int)Precision;     // = 3
                        Debug.WriteLine("[RealPrecisionCommand] Format 1: FloatingPoint64");
                        break;

                    case 2: // CGM Format 2 = Fixed 32-bit
                        Precision = PrecisionType.FixedPoint32Bit;     // = 0
                        CgmContext.RealPrecision = (int)Precision;     // = 0
                        Debug.WriteLine("[RealPrecisionCommand] Format 2: FixedPoint32");
                        break;

                    case 3: // CGM Format 3 = Fixed 64-bit
                        Precision = PrecisionType.FixedPoint64Bit;     // = 1
                        CgmContext.RealPrecision = (int)Precision;     // = 1
                        Debug.WriteLine("[RealPrecisionCommand] Format 3: FixedPoint64");
                        break;

                    default:
                        Debug.WriteLine($"[RealPrecisionCommand WARNING] Unknown format {format}, defaulting to FloatingPoint32");
                        Precision = PrecisionType.FloatingPoint32Bit;
                        CgmContext.RealPrecision = 2;
                        HasReadErrors = true;
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
