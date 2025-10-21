using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public Point2D Position { get; private set; } = new Point2D(0, 0);
        public string Text { get; private set; } = "";

        public TextCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Position du texte
                Position = argReader.MakePoint();
                Debug.WriteLine($"[TextCommand] Read Position=({Position.X}, {Position.Y})");

                // Le texte suit après le point
                Text = argReader.MakeString();
                Debug.WriteLine($"[TextCommand] Position=({Position.X}, {Position.Y}), Text=\"{Text}\"");
                ValidateArgumentsRead("Text");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextCommand ERROR] {ex.Message}");
                Position = new Point2D(0, 0);
                Text = "";
                HasReadErrors = true;
            } 
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Debug.WriteLine("[TextCommand] Pas de texte à dessiner");
                return;
            }

            if (Position == null)
            {
                Debug.WriteLine("[TextCommand] Position invalide");
                return;
            }

            Debug.WriteLine($"[TextCommand] Dessin du texte \"{Text}\" à ({Position.X}, {Position.Y})");

            try
            {
                var position = Position.ToPointF();
                using var brush = new SolidBrush(pen.Color);
                using var font = new Font("Arial", 12); // Taille par défaut

                g.DrawString(Text, font, brush, position);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"TEXT - ({Position.X}, {Position.Y}): \"{Text}\"";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");

        }
    }
}
