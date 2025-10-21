using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// CHARACTER_SPACING (case 13) - Définit le facteur d'espace des caractères (largeur)
    /// </summary>
    public class CharacterSpacingCommand : BaseCgmCommand
    {
        public double Spacing { get; private set; } = 0.0;
        private const double DEFAULT_CHARACTERE_SPACING = 0.0;

        public CharacterSpacingCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterSpacingCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Spacing = argReader.MakeReal();
                Debug.WriteLine($"[CharacterSpacingCommand] {Spacing}");

                ValidateArgumentsRead("CharacterSpacingCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterSpacingCommand Error] {ex.Message}");
                Spacing = DEFAULT_CHARACTERE_SPACING;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTER_SPACING : {Spacing:F4}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
