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
    /// CHARACTER_HEIGHT (case 15) - Définit la hauteur des caractères
    /// </summary>
    public class CharacterHeightCommand : BaseCgmCommand
    {
        public double Height { get; private set; } = 12.0;
        private const double  DEFAULT_HEIGHT = 12.0;

        public CharacterHeightCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterHeightCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Height = argReader.MakeVdc();
                Debug.WriteLine($"[CharacterHeightCommand] Height : {Height:F2}");

                ValidateArgumentsRead("CharacterHeightCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterHeightCommand Error] {ex.Message}");
                Height = DEFAULT_HEIGHT;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTER_HEIGHT : {Height:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
