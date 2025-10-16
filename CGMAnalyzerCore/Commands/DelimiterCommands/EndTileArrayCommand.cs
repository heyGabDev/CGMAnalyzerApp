using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class EndTileArrayCommand : BaseCgmCommand
    {
        public EndTileArrayCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            // Pas d'arguments selon le Java original
            Args = command.Args;
            Debug.WriteLine($"[EndTileArrayCommand] ArgsLength={Args?.Length ?? 0}");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"END_TILE_ARRAY";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
