using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Enums.Core
{
    public enum ControlElement
    {
        Unused0 = 0,
        VdcIntegerPrecision = 1,
        VdcRealPrecision = 2,
        AuxiliaryColour = 3,
        Transparency = 4,
        ClipRectangle = 5,
        ClipIndicator = 6,
        LineClippingMode = 7,
        MarkerClippingMode = 8,
        EdgeClippingMode = 9,
        NewRegion = 10,
        SavePrimitiveContext = 11,
        RestorePrimitiveContext = 12,
        Unused13 = 13,
        Unused14 = 14,
        Unused15 = 15,
        Unused16 = 16,
        ProtectionRegionIndicator = 17,
        GeneralizedTextPathMode = 18,
        MitreLimit = 19,
        TransparentCellColour = 20
    }

    public static class ControlElementExtensions
    {
        public static ControlElement GetElement(int ec)
        {
            if (ec < 0 || ec > 20)
                throw new ArgumentOutOfRangeException(nameof(ec), ec, "Invalid control element code");

            return (ControlElement)ec;
        }
    }
}
