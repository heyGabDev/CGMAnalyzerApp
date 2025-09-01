using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
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
        public Point2D Position { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public string Text { get; private set; } = string.Empty;

        public RestrictedTextCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Rectangle de restriction (largeur et hauteur)
            Width = argReader.MakeVdc();
            Height = argReader.MakeVdc();

            // Position du texte
            Position = argReader.MakePoint(ec, eid);

            // Le texte
            Text = argReader.MakeString();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (string.IsNullOrEmpty(Text)) return;

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

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"RESTRICTED_TEXT at {Position} ({Width}x{Height}): \"{Text}\"";
        }
    }
}
