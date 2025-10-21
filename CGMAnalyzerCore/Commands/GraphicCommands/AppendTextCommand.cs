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
    /// <summary>
    /// APPEND_TEXT (case 6) - Commande non supportée (comme dans la version Java originale)
    /// </summary>
    public class AppendTextCommand : BaseCgmCommand
    {
        public string Text { get; private set; } = "";

        public AppendTextCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[AppendTextCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Le texte à ajouter
                Text = argReader.MakeString();
                Debug.WriteLine($"[AppendTextCommand] Text=\"{Text}\"");
                ValidateArgumentsRead("AppendText");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppendTextCommand ERROR] {ex.Message}");
                Text = "";
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"APPEND_TEXT: \"{Text}\"";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
