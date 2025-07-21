using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter.Enum
{
    public enum GraphicalPrimitiveElement
    {
        Polyline = 1,
        DisjointPolyline = 2,
        PolyMarker = 3,
        Text = 4,
        RestrictedText = 5,
        AppendText = 6,
        Polygon = 7,
        PolygonSet = 8,
        CellArray = 9,
        GENERALIZED_DRAWING_PRIMITIVE = 10,
        Rectangle = 11,
        Circle = 12,
        CircularArc3Point = 13,
        CircularArc3PointClose = 14,
        CircularArcCentre = 15,
        CircularArcCentreClose = 16,
        Ellipse = 17,
        EllipticalArc = 18,
        EllipticalArcClose = 19,
        CIRCULAR_ARC_CENTRE_REVERSED = 20,
		CONNECTING_EDGE = 21,
		HYPERBOLIC_ARC = 22,
		PARABOLIC_ARC = 23,
		NON_UNIFORM_B_SPLINE = 24,
		NON_UNIFORM_RATIONAL_B_SPLINE = 25,
        PolyBezier = 26,
        BitonalTile = 28,
        Tile = 29
    }
}
