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
    public class AlternateCharacterSetIndexCommand : BaseCgmCommand
    {
        /// <summary>
        /// ALTERNATE_CHARACTER_SET_INDEX (case 20) - Définit l'index du jeu de caractères alternatif
        /// </summary>
        public int AlternateCharacterSetIndex { get; private set; } = 1;
        private const int DEFAULT_ALTERNATE_CHARACTER_SET_INDEX = 1;    
        public AlternateCharacterSetIndexCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[AlternateCharacterSetIndexCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                AlternateCharacterSetIndex = argReader.MakeIndex();
                Debug.WriteLine($"[AlternateCharacterSetIndexCommand] AlternateCharacterSetIndex: {AlternateCharacterSetIndex}");

                ValidateArgumentsRead("AlternateCharacterSetIndexCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AlternateCharacterSetIndexCommand Error] {ex.Message}");
                AlternateCharacterSetIndex = DEFAULT_ALTERNATE_CHARACTER_SET_INDEX;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"ALTERNATIVE_CHARACTER_SET_INDEX : {AlternateCharacterSetIndex}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
