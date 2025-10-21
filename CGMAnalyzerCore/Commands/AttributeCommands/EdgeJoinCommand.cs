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
    /// EDGE_JOIN (case 45) - Définit le style de jonction des bords
    /// </summary>

    public enum EdgeJoinType
    {
        Unspecified = 1,
        Mitre = 2,
        Round = 3,
        Bevel = 4
    }

    public class EdgeJoinCommand : BaseCgmCommand
    {
        public EdgeJoinType EdgeJoin { get; private set; } = EdgeJoinType.Unspecified;
        private const EdgeJoinType DEFAULT_EDGE_JOIN_TYPE = EdgeJoinType.Unspecified;

        public EdgeJoinCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeJoinCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int edgeJoinValue = argReader.MakeIndex();
                EdgeJoin = edgeJoinValue switch
                {
                    1 => EdgeJoinType.Unspecified,
                    2 => EdgeJoinType.Mitre,
                    3 => EdgeJoinType.Round,
                    4 => EdgeJoinType.Bevel,
                    _ => EdgeJoinType.Unspecified
                };
                Debug.WriteLine($"[EdgeJoinCommand] EdgeJoinType={edgeJoinValue} : {EdgeJoin}");

                ValidateArgumentsRead("EdgeJoinCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeJoinCommand Error] {ex.Message}");
                EdgeJoin = DEFAULT_EDGE_JOIN_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_JOIN : {EdgeJoin}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
