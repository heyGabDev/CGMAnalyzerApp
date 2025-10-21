using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// TEXT_FONT_INDEX (case 10) - Définit l'index de la police de texte
    /// </summary>
    public class TextFontIndexCommand : BaseCgmCommand
    {
        public int FontIndex { get; private set; } = 1;
        private const int DEFAULT_FONT_INDEX = 1;
        public TextFontIndexCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextFontIndexCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                FontIndex = argReader.MakeIndex();
                Debug.WriteLine($"[TextFontIndexCommand] {FontIndex}");
                ValidateArgumentsRead("TextFontIndexCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextFontIndexCommand Error] {ex.Message}");
                FontIndex = DEFAULT_FONT_INDEX;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"TEXT_FONT_INDEX: {FontIndex}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
