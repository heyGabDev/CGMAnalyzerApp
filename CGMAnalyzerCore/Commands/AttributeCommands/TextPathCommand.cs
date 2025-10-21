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
    /// TEXT_PATH (case 17) - Définit la direction d'écriture du texte
    /// </summary>
    public enum TextPathType
    {
        Right = 0,
        Left = 1,
        Up = 2,
        Down = 3
    }

    public class TextPathCommand : BaseCgmCommand
    {
        public TextPathType Path { get; private set; } = TextPathType.Right;
        private const TextPathType DEFAULT_TEXT_PATH_TYPE = TextPathType.Right;

        public TextPathCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextPathCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                int pathValue = argReader.MakeEnum();
                Path = pathValue switch
                {
                    0 => TextPathType.Right,
                    1 => TextPathType.Left,
                    2 => TextPathType.Up,
                    3 => TextPathType.Down,
                    _ => TextPathType.Right
                };
                Debug.WriteLine($"[TextPathCommand] {pathValue}");

                ValidateArgumentsRead("TextPathCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextPathCommand Error] {ex.Message}");
                Path = DEFAULT_TEXT_PATH_TYPE;
                HasReadErrors = true;
            }
        }


        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"TEXT_PATH : {Path}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
