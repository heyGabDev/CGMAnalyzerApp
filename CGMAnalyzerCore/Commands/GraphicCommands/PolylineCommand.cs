using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// POLYMARKER (case 1) - Dessine une ligne continue passant par plusieurs points
    /// </summary>
    public class PolylineCommand : BaseCgmCommand
    {
        public List<Point2D.Double> Points { get; private set; } = new List<Point2D.Double>();

        public PolylineCommand(int ec, int eid, int l, BinaryReader reader)
             : base(ec, eid, l)
        {
            var command = new CgmCommand(ec, eid, l, reader);
            Args = command.Args;

            Debug.WriteLine($"[PolylineCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command); // 'this' pas 'reader'
                int pointCount = 0;
                while (CurrentArg < Args?.Length)
                {
                    Point2D.Double point = argReader.MakePoint();
                    Points.Add(point);
                    pointCount++;
                    Debug.WriteLine($"[PolylineCommand] Point {pointCount}: ({point.X}, {point.Y})");
                }

                Debug.WriteLine($"[PolylineCommand] Total points: {Points.Count}");
                ValidateArgumentsRead("PolylineCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolylineCommand ERROR] {ex.Message}");
                Points = new List<Point2D.Double>();
                HasReadErrors = true;
            }
            //// Lire les arguments dans Args[] via le constructeur parent
            //var command = new CgmCommand(ec, eid, l, reader);
            //Args = command.Args;

            //// Parser avec ExtractedArgumentReader
            //var argReader = new ExtractedArgumentReader(command);

            //int pointSize = argReader.SizeOfPoint();
            //int pointCount = command.RemainingArgs() / pointSize;

            //Debug.WriteLine($"[PolylineCommand] ArgsLength={Args.Length} PointSize={pointSize} PointCount={pointCount}");

            //for (int i = 0; i < pointCount; i++)
            //{
            //    var point = argReader.MakePoint(ec, eid);
            //    Points.Add(point);
            //    Debug.WriteLine($"[PolylineCommand] Point {i}: ({point.X}, {point.Y})");
            //}

            //ValidateArgumentsRead("PolylineCommand");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Points.Count < 2)
            {
                Debug.WriteLine($"[PolylineCommand] Pas assez de points pour dessiner: {Points.Count}");
                return;
            }

            var pointsArray = new PointF[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                pointsArray[i] = new PointF((float)Points[i].X, (float)Points[i].Y);
            }

            g.DrawLines(pen, pointsArray);
            Debug.WriteLine($"[PolylineCommand] Dessiné {Points.Count} points");
        }

        public override string ToString()
        {
            return $"POLYLINE ({Points.Count} points)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
