using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Context
{
    public static class CgmContext
    /// <summary>
    /// Contexte global de parsing CGM : valeurs partagées comme le Layer courant.
    /// Peut être étendu avec d'autres états graphiques (style, couleurs, font...).
    /// </summary>
    {
        // ---- VDC type: Integer or Real
        public static VDCTypeEnum VdcType { get; set; } = VDCTypeEnum.Integer;

        // ---- Precision for integer VDCs (in bits)
        public static int VdcIntegerPrecision { get; set; } = 16;

        // ---- Precision for real VDCs
        public static VDCRealPrecisionEnum VdcRealPrecision { get; set; } = VDCRealPrecisionEnum.FixedPoint32;

        // ---- Layer
        public static int CurrentLayerId { get; set; } = 0;

        public static int IntegerPrecision { get; set; } = 16;

        public static int IndexPrecision { get; set; } = 16;
        public static int NamePrecision { get; set; } = 16;
        public static int ColorPrecision { get; set; } = 8;
        public static int ColorIndexPrecision { get; set; } = 8;
        public static int RealPrecision { get; set; } = 32; // Fixed32 par défaut

        public static void Reset()
        {
            VdcType = VDCTypeEnum.Integer;
            VdcIntegerPrecision = 16;
            VdcRealPrecision = VDCRealPrecisionEnum.FixedPoint32;
            CurrentLayerId = 0;
            IntegerPrecision = 16;
            IndexPrecision = 16;
            NamePrecision = 16;
            ColorPrecision = 8;
            ColorIndexPrecision = 8;
            RealPrecision = 32;
        }
    }

    public enum VDCTypeEnum
    {
        Integer = 0,
        Real = 1
    }

    public enum VDCRealPrecisionEnum
    {
        FixedPoint32 = 0,
        FixedPoint64 = 1,
        FloatingPoint32 = 2,
        FloatingPoint64 = 3,
    }
}
