using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// LINE_JOIN (case 38) - Définit le style de jonction des lignes
    /// </summary>

    public enum LineJoinType
    {
        Unspecified = 1,
        Mitre = 2,
        Round = 3,
        Bevel = 4
    }

    public class LineJoinCommand : BaseCgmCommand
    {
        public LineJoinType LineJoin { get; private set; } = LineJoinType.Unspecified;
        private const LineJoinType DEFAULT_LINE_JOIN_TYPE = LineJoinType.Unspecified;

        public LineJoinCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineJoinCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                int joinValue = argReader.MakeIndex();
                LineJoin = joinValue switch
                {
                    1 => LineJoinType.Unspecified,
                    2 => LineJoinType.Mitre,
                    3 => LineJoinType.Round,
                    4 => LineJoinType.Bevel,
                    _ => LineJoinType.Unspecified
                };
                Debug.WriteLine($"[LineJoinCommand] LineJoinType={joinValue} :{LineJoin}");

                ValidateArgumentsRead("LineJoinCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineJoinCommand Error] {ex.Message}");
                LineJoin = DEFAULT_LINE_JOIN_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_JOIN : {LineJoin}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
