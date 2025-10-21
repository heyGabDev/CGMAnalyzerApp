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
    /// CHARACTER_ORIENTATION (case 16) - Définit l'orientation des caractères (vecteurs up et base)
    /// </summary>
    public class CharacterOrientationCommand : BaseCgmCommand
    {
        public double XUp { get; private set; } = 0.0;
        public double YUp { get; private set; } = 1.0;
        public double XBase { get; private set; } = 1.0;
        public double YBase { get; private set; } = 0.0;

        public CharacterOrientationCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterOrientationCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                XUp = argReader.MakeVdc();
                YUp = argReader.MakeVdc();
                XBase = argReader.MakeVdc();
                YBase = argReader.MakeVdc();
                Debug.WriteLine($"[CharacterOrientationCommand] Up vector: ({XUp:F2}, {YUp:F2}), Base vector: ({XBase:F2}, {YBase:F2})");

                ValidateArgumentsRead($"CharacterOrientationCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterOrientationCommand Error] {ex.Message}");
                XUp = 0.0;
                YUp = 1.0;
                XBase = 1.0;
                YBase = 0.0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTER_ORIENTATION: up=({XUp:F2}, {YUp:F2}) base=({XBase:F2}, {YBase:F2})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
