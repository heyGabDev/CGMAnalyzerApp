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
    /// CHARACTER_SET_INDEX (case 19) - Définit l'index du jeu de caractères
    /// </summary>
    public class CharacterSetIndexCommand : BaseCgmCommand
    {
        public int CharacterSetIndex { get; private set; } = 1;
        private const int DEFAULT_CHARACTER_SET_INDEX = 1;

        public CharacterSetIndexCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterSetIndexCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                CharacterSetIndex = argReader.MakeIndex();
                Debug.WriteLine($"[CharacterSetIndexCommand] CharacterSetIndex: {CharacterSetIndex}");

                ValidateArgumentsRead("CharacterSetIndex");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterSetIndexCommand Error] {ex.Message}");
                CharacterSetIndex=DEFAULT_CHARACTER_SET_INDEX;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTER_SET_INDEX : {CharacterSetIndex}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
