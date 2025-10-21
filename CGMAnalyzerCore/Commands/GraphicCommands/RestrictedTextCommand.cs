using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// RESTRICTED_TEXT (case 5) - Texte dans une zone rectangulaire définie
    /// </summary>
    public class RestrictedTextCommand : BaseCgmCommand
    {
        public Point2D Position { get; private set; } = new Point2D(0, 0);
        public double Width { get; private set; } = 0;
        public double Height { get; private set; } = 0;
        public string Text { get; private set; } = "";

        public RestrictedTextCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[RestrictedTextCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Rectangle de restriction (largeur et hauteur)
                Width = argReader.MakeVdc();
                Debug.WriteLine($"[RestrictedTextCommand] Read Width={Width}");

                Height = argReader.MakeVdc();
                Debug.WriteLine($"[RestrictedTextCommand] Read Height={Height}");

                // Position du texte
                Position = argReader.MakePoint();
                Debug.WriteLine($"[RestrictedTextCommand] Read Position=({Position.X}, {Position.Y})");

                // Le texte
                Text = argReader.MakeString();
                Debug.WriteLine($"[RestrictedTextCommand] Position=({Position.X}, {Position.Y}), Width={Width}, Height={Height}, Text=\"{Text}\"");
                
                ValidateArgumentsRead("RestrictedText");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RestrictedTextCommand ERROR] {ex.Message}");
                Position = new Point2D(0, 0);
                Width = 0;
                Height = 0;
                Text = "";
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Debug.WriteLine("[RestrictTextCommand] Pas de texte à dessiner");
                return;
            }

            if(Position == null)
            {
                Debug.WriteLine("[RestrictTextCommand] Position invalide");
                return;
            }

            Debug.WriteLine($"[RestrictTextCommand] Dessin du texte \"{Text}\" à ({Position.X}, {Position.Y}) avec largeur {Width} et hauteur {Height}");

            try
            {
                var position = Position.ToPointF();
                using var brush = new SolidBrush(pen.Color);
                using var font = new Font("Arial", 10); // Plus petit pour le texte restreint

                // Créer un rectangle de restriction
                var restrictRect = new RectangleF(
                    position.X, position.Y,
                    (float)Width, (float)Height
                );

                // Dessiner le texte dans le rectangle avec troncature
                var stringFormat = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                g.DrawString(Text, font, brush, restrictRect, stringFormat);

                // Optionnel : dessiner le rectangle de restriction en pointillés
                using var restrictPen = new Pen(Color.LightGray) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                g.DrawRectangle(restrictPen, Rectangle.Round(restrictRect));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RestrictTextCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }     
        }

        public override string ToString()
        {
            return $"RESTRICTED_TEXT - ( POSITION : {Position.X}, {Position.Y}, LARGEUR : {Width}x HAUTEUR {Height}): \"{Text}\"";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");

        }
    }
}
