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
    public class CharacterExpansionFactorCommand : BaseCgmCommand
    {
        /// <summary>
        /// CHARACTER_EXPANSION_FACTOR (case 12) - Définit le facteur d'expansion des caractères (largeur)
        /// </summary>
        public double ExpansionFactor { get; private set; } = 1.0;
        public const double DEFAULT_EXPANSION_FACTOR = 1.0;
        public CharacterExpansionFactorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterExpansionFactorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                ExpansionFactor = argReader.MakeReal();
                Debug.WriteLine($"[CharacterExpansionFactorCommand] {ExpansionFactor}");

                ValidateArgumentsRead("ExpansionFactor");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterExpansionFactorCommand ERROR] {ex.Message}");
                ExpansionFactor = 1.0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTER_EXPANSION_FACTOR : {ExpansionFactor : F4}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
