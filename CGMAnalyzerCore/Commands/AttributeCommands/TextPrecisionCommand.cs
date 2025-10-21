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
    public class TextPrecisionCommand : BaseCgmCommand
    {
        /// <summary>
        /// TEXT_PRECISION (case 11) - Définit la précision du rendu de texte
        /// </summary>
        public enum TextPrecisionType
        {
            String = 0,
            Character = 1,
            Stroke = 2
        }

        public TextPrecisionType Precision { get; private set; } = TextPrecisionType.String;
        private const TextPrecisionType DEFAULT_TEXT_PRECISION_TYPE = TextPrecisionType.String;

        public TextPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                int precisionValue = argReader.MakeEnum();
                Precision = precisionValue switch
                {
                    0 => TextPrecisionType.String,
                    1 => TextPrecisionType.Character,
                    2 => TextPrecisionType.Stroke,
                    _ => TextPrecisionType.String
                };

                Debug.WriteLine($"[TextPrecisionCommand] Precision : {precisionValue}");
                ValidateArgumentsRead("TextPrecisionCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TextPrecisionCommand Error] {ex.Message}");
                Precision = DEFAULT_TEXT_PRECISION_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"TEXT_PRECISION: {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
