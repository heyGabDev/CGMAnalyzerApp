using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// RECTANGLE (case 11) - Dessine un rectangle défini par deux coins opposés
    /// </summary>
    public class RectangleCommand : BaseCgmCommand
    {
        public Point2D Corner1 { get; private set; } = new Point2D(0, 0);
        public Point2D Corner2 { get; private set; } = new Point2D(0, 0);

        public RectangleCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[RectangleCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                // Un rectangle est défini par 2 points (coins opposés)
                Corner1 = argReader.MakePoint();
                Debug.WriteLine($"[RectangleCommand] Read Corner1=({Corner1.X}, {Corner1.Y})");

                Corner2 = argReader.MakePoint();
                Debug.WriteLine($"[RectangleCommand] Read Corner2=({Corner2.X}, {Corner2.Y})");

                ValidateArgumentsRead("RectangleCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RectangleCommand ERROR] {ex.Message}");
                Corner1 = new Point2D(0, 0);
                Corner2 = new Point2D(0, 0);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Corner1 == null || Corner2 == null)
            {
                Debug.WriteLine("[RectangleCommand] Coins invalides");
                return;
            }

            Debug.WriteLine($"[RectangleCommand] Dessin du rectangle " +
                            $"entre ({Corner1.X}, {Corner1.Y}) et ({Corner2.X}, {Corner2.Y})");

            try
            {
                // Calculer les coordonnées du rectangle
                float x = (float)Math.Min(Corner1.X, Corner2.X);
                float y = (float)Math.Min(Corner1.Y, Corner2.Y);
                float width = (float)Math.Abs(Corner2.X - Corner1.X);
                float height = (float)Math.Abs(Corner2.Y - Corner1.Y);
                Debug.WriteLine($"[RectangleCommand] Dessin du rectangle " +
                                $"en ({x}, {y}) avec largeur {width} et hauteur {height}");

                if (width > 0 && height > 0)
                {
                    g.DrawRectangle(pen, x, y, width, height);
                }
                else
                {
                    Debug.WriteLine($"[RectangleCommand] Dimensions invalides: {width}x{height}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RectangleCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"RECTANGLE de ({Corner1.X}, {Corner1.Y}) à ({Corner2.X}, {Corner2.Y})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
