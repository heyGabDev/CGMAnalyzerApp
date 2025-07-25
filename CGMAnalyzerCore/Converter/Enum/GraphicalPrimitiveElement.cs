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
        GeneralizedDrawingPrimitive = 10,
        Rectangle = 11,
        Circle = 12,
        CircularArc3Point = 13,
        CircularArc3PointClose = 14,
        CircularArcCentre = 15,
        CircularArcCentreClose = 16,
        Ellipse = 17,
        EllipticalArc = 18,
        EllipticalArcClose = 19,
        CircularArcCentreReversed = 20,
        ConnectingEdge = 21,
        HyperbolicArc = 22,
        ParabolicArc = 23,
        NonUniformBSpline = 24,
        NonUniformRationalBSpline = 25,
        PolyBezier = 26,
        BitonalTile = 28,
        Tile = 29
    }
}
