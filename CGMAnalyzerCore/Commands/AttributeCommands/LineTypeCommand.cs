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
    /// LINE_TYPE (case 2) - Définit le type de ligne (solide, pointillé, etc.)
    /// </summary>
    public class LineTypeCommand : BaseCgmCommand
    {
        public int LineType { get; private set; } = 1;
        private const int DEFAULT_LINE_TYPE = 1;
        public LineTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                LineType = argReader.MakeIndex();
                Debug.WriteLine($"[LineTypeCommand] {LineType} ({GetLineTypeName(LineType)})");
                ValidateArgumentsRead("LineType");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineTypeCommand Error] {ex.Message}");
                LineType = DEFAULT_LINE_TYPE;
                HasReadErrors = true;
            }
        }

        private string GetLineTypeName(int type)
        {
            return type switch
            {
                1 => "Solid",
                2 => "Dash",
                3 => "Dot",
                4 => "Dash-dot",
                5 => "Dash-dot-dot",
                _ => $"Custom ({type})"
            };
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_TYPE {LineType} ({GetLineTypeName(LineType)})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
