using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class FontListCommand : BaseCgmCommand
    {
        public string[] FontNames { get; private set; }

        public FontListCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[FontListCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                var fontList = new List<string>();

                // Lire les chaînes jusqu'à épuisement des arguments
                while (command.CurrentArg < Args?.Length)
                {
                    string fontName = argReader.MakeString();
                    fontList.Add(fontName);
                    Debug.WriteLine($"[FontListCommand] Font[{fontList.Count - 1}]: {fontName}");
                }

                FontNames = fontList.ToArray();
                Debug.WriteLine($"[FontListCommand] Total fonts: {FontNames.Length}");
                ValidateArgumentsRead("FontListCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FontListCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            if (FontNames == null || FontNames.Length == 0)
                return "FONT_LIST: (empty)";

            return $"FONT_LIST: {string.Join(", ", FontNames)}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
