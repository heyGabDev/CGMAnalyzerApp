using CGMAnalyzerCore.Enums.Colors;
using CGMAnalyzerCore.Enums.Precision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.ColourSelectionModeCommand;
using static CGMAnalyzerCore.Commands.PictureCommands.DeviceViewportSpecificationModeCommand;

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

        // ---- Precision settings
        public static int IntegerPrecision { get; set; } = 16;
        public static int IndexPrecision { get; set; } = 16;
        public static int NamePrecision { get; set; } = 16;
        public static int RealPrecision { get; set; } = 0; // 0 = Fixed32 par défaut

        // ---- Colour settings 
        public static int ColorIndexPrecision { get; set; } = 8;
        public static int ColorPrecision { get; set; } = 8;
        public static int[] MinimumColorValueRGB { get; set; } = new int[] { 0, 0, 0 };
        public static int[] MaximumColorValueRGB { get; set; } = new int[] { 255, 255, 255 };
        public static ColorModelEnum ColourModel { get; set; } = ColorModelEnum.RGB;

        // ---- Picture Descriptor settings (nouvelles propriétés)
        public static ColorSelectionType ColorSelectionMode { get; set; } = ColorSelectionType.INDEXED;
        public static SpecificationMode LineWidthSpecificationMode { get; set; } = SpecificationMode.ABSOLUTE;
        public static SpecificationMode MarkerSizeSpecificationMode { get; set; } = SpecificationMode.ABSOLUTE;
        public static SpecificationMode EdgeWidthSpecificationMode { get; set; } = SpecificationMode.ABSOLUTE;
        public static DeviceViewportMode DeviceViewportSpecificationMode { get; set; } = DeviceViewportMode.FractionOfDrawingSurface;

        public static void Reset()
        {
            VdcType = VDCTypeEnum.Integer;
            VdcIntegerPrecision = 16;
            VdcRealPrecision = VDCRealPrecisionEnum.FixedPoint32;
            CurrentLayerId = 0;
            IntegerPrecision = 16;
            IndexPrecision = 16;
            NamePrecision = 16;
            RealPrecision = 0; // Fixed32
            ColorIndexPrecision = 8;
            ColorPrecision = 8;
            ColourModel = ColorModelEnum.RGB;
            MinimumColorValueRGB = new int[] { 0, 0, 0 };
            MaximumColorValueRGB = new int[] { 255, 255, 255 };

            // Reset Picture Descriptor settings
            ColorSelectionMode = ColorSelectionType.INDEXED;
            LineWidthSpecificationMode = SpecificationMode.ABSOLUTE;
            MarkerSizeSpecificationMode = SpecificationMode.ABSOLUTE;
            EdgeWidthSpecificationMode = SpecificationMode.ABSOLUTE;
            DeviceViewportSpecificationMode = DeviceViewportMode.FractionOfDrawingSurface;
        }
    }
}
