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
    // <summary>
    /// TEXT (case 4) - Dessine du texte à une position donnée
    /// </summary>
    public class TextCommand : BaseCgmCommand
    {
        public Point2D Position { get; private set; }
        public string Text { get; private set; } = string.Empty;

        public TextCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Position du texte
            Position = argReader.MakePoint(ec, eid);

            // Le texte suit après le point
            Text = argReader.MakeString();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (string.IsNullOrEmpty(Text)) return;

            var position = Position.ToPointF();
            using var brush = new SolidBrush(pen.Color);
            using var font = new Font("Arial", 12); // Taille par défaut

            g.DrawString(Text, font, brush, position);
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"TEXT at {Position}: \"{Text}\"";
        }
    }
}
