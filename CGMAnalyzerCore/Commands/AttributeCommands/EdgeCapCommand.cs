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
    /// EDGE_CAP (case 44) - Définit le style de terminaison des bords
    /// </summary>
    public enum EdgeCapType
    {
        Unspecified = 1,
        Butt = 2,
        Round = 3,
        ProjectingSquare = 4,
        Triangle = 5
    }

    public class EdgeCapCommand : BaseCgmCommand
    {
        public EdgeCapType EdgeCap { get; private set; } = EdgeCapType.Unspecified;
        private const EdgeCapType DEFAULT_EDGE_CAP_TYPE = EdgeCapType.Unspecified;

        public EdgeCapCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeCapCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int edgeCapValue = argReader.MakeIndex();
                EdgeCap = edgeCapValue switch
                {
                    1 => EdgeCapType.Unspecified,
                    2 => EdgeCapType.Butt,
                    3 => EdgeCapType.Round,
                    4 => EdgeCapType.ProjectingSquare,
                    5 => EdgeCapType.Triangle,
                    _ => EdgeCapType.Unspecified
                };
                Debug.WriteLine($"[EdgeCapCommand] EdgeCapType={edgeCapValue} : {EdgeCap}");

                ValidateArgumentsRead("EdgeCapCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeCapCommand Error] {ex.Message}");
                EdgeCap = DEFAULT_EDGE_CAP_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_CAP : {EdgeCap}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
