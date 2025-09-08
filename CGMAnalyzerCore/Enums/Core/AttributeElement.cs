using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Enums.Core
{
    public enum AttributeElement
    {
        LineBundleIndex = 1,
        LineType = 2,
        LineWidth = 3,
        //LineWidthSpecMode = 4,
        LineColour = 4,
        MarkerBundleIndex = 5,
        MarkerType = 6,
        MarkerSize = 7,
        //MarkerSizeSpecMode = 9,
        MarkerColour = 8,
        TextBundleIndex = 9,
        TextFontIndex = 10,
        TextPrecision = 11,
        CharacterExpansionFactor = 12,
        CharacterSpacing = 13,
        TextColour = 14,
        CharacterHeight = 15,
        CharacterOrientation = 16,
        TextPath = 17,
        TextAlignment = 18,
        CharacterSetIndex = 19, 
        AlternateCharacterSetIndex = 20,
        FillBundleIndex = 21,
        InteriorStyle = 22,
        FillColour = 23,
        HatchIndex = 24,
        PatternIndex = 25,
        EdgeBundleIndex = 26,
        EdgeType = 27,
        EdgeWidth = 28,
        //EdgeWidthSpecMode = 31,
        EdgeColour = 29,
        EdgeVisibility = 30,

        //or
        FillReferencePoint = 31,
        PatternTable = 32,
        PatternSize = 33,
        ColourTable = 34,

        // or
        AspectSourceFlags = 35,
        PickIdentifier = 36,
        LineCap = 37,
        LineJoin = 38,

        //or
        LineTypeContinuation = 39, 
        LineTypeInitialOffset = 40,
        TextScoreType = 41,
        RestrictedTextType = 42,
        InterpolatedInterior = 43,
        EdgeCap = 44,
        EdgeJoin = 45,

        //or
        EdgeTypeContinuation = 46,
        EdgeTypeInitialOffset = 47,
        SymbolLibraryIndex = 48,
        SymbolColour = 49,
        SymbolSize = 50,
        SymbolOrientation = 51,

        //LineDashOffset = 43,//unsupported, use new CgmCommand()
        //LineDashScaleMode = 44,
        //LineDashCap = 45,
        //LineMiterLimit = 46,
        //MarkerTypeContinuation = 47,
        //MarkerSizeContinuation = 48,
        //MarkerScaleMode = 49,
        //CharacterSetList = 50,
        //FontList = 51,
        //TextScoreIndex = 52,
        //TextScoreContinuation = 53,
        //FillStyle = 54,
        //EdgeStyle = 55
    }

    public static class AttributeElementHelper
    {
        private static readonly Dictionary<int, AttributeElement> _map = new();

        static AttributeElementHelper()
        {
            // Enum.GetValues
            foreach (AttributeElement element in Enum.GetValues(typeof(AttributeElement)))
            {
                _map[(int)element] = element;
            }
        }

        public static AttributeElement? GetElement(int id)
        {
            return _map.TryGetValue(id, out var result) ? result : null;
        }
    }
}
