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
        // <summary>
        /// Extensions pour LineCommand
        /// </summary>
        public static List<Point> GetPoints(this LineCommand command)
        {
            // À implémenter selon ta structure de données
            // Exemple basique :
            return command.Arguments?.Take(4).ToArray() switch
            {
                var args when args.Length >= 4 => new List<Point>
                {
                    new Point((int)args[0], (int)args[1]),
                    new Point((int)args[2], (int)args[3])
                },
                _ => new List<Point>()
            };
        }

        /// <summary>
        /// Extensions pour PolylineCommand
        /// </summary>
        public static List<Point> GetPoints(this PolylineCommand command)
        {
            var points = new List<Point>();
            var args = command.Arguments;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length - 1; i += 2)
                {
                    points.Add(new Point((int)args[i], (int)args[i + 1]));
                }
            }

            return points;
        }

        /// <summary>
        /// Extensions pour DisjointPolylineCommand
        /// </summary>
        public static List<List<Point>> GetPointSets(this DisjointPolylineCommand command)
        {
            // Implémentation à adapter selon ta logique
            var pointSets = new List<List<Point>>();
            var args = command.Arguments;

            if (args?.Length >= 4)
            {
                // Exemple : supposons que les points sont groupés par paires
                var currentSet = new List<Point>();

                for (int i = 0; i < args.Length - 1; i += 2)
                {
                    currentSet.Add(new Point((int)args[i], (int)args[i + 1]));

                    // Logique pour détecter une nouvelle séquence (à adapter)
                    if (currentSet.Count >= 2)
                    {
                        pointSets.Add(new List<Point>(currentSet));
                        currentSet.Clear();
                    }
                }

                if (currentSet.Count >= 2)
                    pointSets.Add(currentSet);
            }

            return pointSets;
        }

        /// <summary>
        /// Extensions pour EllipticalArcCommand
        /// </summary>
        public static Rectangle? GetBounds(this EllipticalArcCommand command)
        {
            var args = command.Arguments;
            if (args?.Length >= 4)
            {
                return new Rectangle(
                    (int)args[0], (int)args[1],
                    (int)args[2], (int)args[3]
                );
            }
            return null;
        }

        public static float GetStartAngle(this EllipticalArcCommand command)
        {
            var args = command.Arguments;
            return args?.Length > 4 ? (float)args[4] : 0f;
        }

        public static float GetSweepAngle(this EllipticalArcCommand command)
        {
            var args = command.Arguments;
            return args?.Length > 5 ? (float)args[5] : 360f;
        }

        /// <summary>
        /// Extensions pour les commandes de métadonnées
        /// </summary>
        public static string GetColourModel(this ColourModelCommand command)
        {
            return command.Arguments?.FirstOrDefault()?.ToString() ?? "RGB";
        }

        public static int GetPrecision(this IntegerPrecisionCommand command)
        {
            return command.Arguments?.FirstOrDefault() is double precision ? (int)precision : 16;
        }

        public static Rectangle GetExtent(this MaximumVdcExtentCommand command)
        {
            var args = command.Arguments;
            if (args?.Length >= 4)
            {
                return new Rectangle(
                    (int)args[0], (int)args[1],
                    (int)(args[2] - args[0]), (int)(args[3] - args[1])
                );
            }
            return new Rectangle(0, 0, 32767, 32767);
        }

        public static string? GetVersion(this MetafileVersionCommand command)
        {
            return command.Arguments?.FirstOrDefault()?.ToString();
        }
    }
}

    
}
