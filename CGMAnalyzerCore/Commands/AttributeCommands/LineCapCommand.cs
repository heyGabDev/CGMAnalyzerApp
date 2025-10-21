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
    /// LINE_CAP (case 37) - Définit le style de terminaison des lignes
    /// </summary>
    public enum LineCapType
    {
        Unspecified = 1,
        Butt = 2,
        Round = 3,
        ProjectingSquare = 4,
        Triangle = 5
    }

    public class LineCapCommand : BaseCgmCommand
    {
        public LineCapType LineCap { get; private set; } = LineCapType.Unspecified;
        private const LineCapType DEFAULT_LINE_CAP_TYPE = LineCapType.Unspecified;

        public LineCapCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineCapCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int capValue = argReader.MakeIndex();
                LineCap = capValue switch
                {
                    1 => LineCapType.Unspecified,
                    2 => LineCapType.Butt,
                    3 => LineCapType.Round,
                    4 => LineCapType.ProjectingSquare,
                    5 => LineCapType.Triangle,
                    _ => LineCapType.Unspecified
                };
                Debug.WriteLine($"[LineCapCommand] LineCapType={capValue} : {LineCap}");

                ValidateArgumentsRead("LineCapCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineCapCommand Error] {ex.Message}");
                LineCap = DEFAULT_LINE_CAP_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_CAP : {LineCap}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
