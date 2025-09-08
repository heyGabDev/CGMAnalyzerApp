using CGMAnalyzerCore.Commands.MetafileCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Extension pour TES LineCommand qui utilisent Start/End
        /// </summary>
        public static List<Point> GetPoints(this LineCommand command)
        {
            return new List<Point> { command.Start, command.End };
        }

        /// <summary>
        /// Extension pour TES PolylineCommand qui ont une propriété Points
        /// </summary>
        public static List<Point> GetPoints(this PolylineCommand command)
        {
            return command.Points ?? new List<Point>();
        }

        /// <summary>
        /// Extension pour TES DisjointPolylineCommand qui utilisent Point2D
        /// </summary>
        public static List<List<Point>> GetPointSets(this DisjointPolylineCommand command)
        {
            var pointSets = new List<List<Point>>();

            foreach (var line in command.Lines)
            {
                var linePoints = new List<Point>
                {
                    // Convertir Point2D vers Point standard
                    new Point((int)line.Start.X, (int)line.Start.Y),
                    new Point((int)line.End.X, (int)line.End.Y)
                };
                pointSets.Add(linePoints);
            }

            return pointSets;
        }

        /// <summary>
        /// Extension pour TES EllipticalArcCommand 
        /// </summary>
        public static Rectangle? GetBounds(this EllipticalArcCommand command)
        {
            // Tu n'exposes pas les propriétés privées, donc on utilise une estimation
            // Idéalement, tu devrais ajouter des propriétés publiques dans EllipticalArcCommand

            // Pour le moment, retourner un rectangle par défaut
            // Tu peux modifier EllipticalArcCommand pour exposer Center, etc.
            return new Rectangle(100, 100, 200, 150); // Rectangle par défaut
        }

        public static float GetStartAngle(this EllipticalArcCommand command)
        {
            // Pareil ici, tu n'exposes pas _startAngle
            // Idéalement, ajouter une propriété publique StartAngle
            return 0f; // Valeur par défaut
        }

        public static float GetSweepAngle(this EllipticalArcCommand command)
        {
            // Pareil pour _extentAngle
            return 360f; // Valeur par défaut
        }

        /// <summary>
        /// Extensions pour les commandes de métadonnées - versions basiques
        /// </summary>
        public static string GetColourModel(this ColorModelCommand command)
        {
            return "RGB"; // Par défaut
        }

        public static int GetPrecision(this IntegerPrecisionCommand command)
        {
            return 16; // Par défaut
        }

        public static string GetVersion(this MetafileVersionCommand command)
        {
            return "1.0"; // Par défaut
        }

        public static Rectangle GetExtent(this MaximumVdcExtentCommand command)
        {
            // Rectangle par défaut si pas d'accès aux données internes
            return new Rectangle(0, 0, 32767, 32767);
        }
    }


}
