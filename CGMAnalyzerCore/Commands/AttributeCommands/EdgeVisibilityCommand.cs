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
    /// EDGE_VISIBILITY (case 30) - Définit 
    /// </summary>
    public class EdgeVisibilityCommand : BaseCgmCommand
    {
        public bool EdgeVisible { get; private set; } = false;
        private const bool DEFAULT_EDGE_VISIBLE = false;
        public EdgeVisibilityCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeVisibilityCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                EdgeVisible = argReader.MakeEnum() == 1;
                Debug.WriteLine($"[EdgeVisibilityCommand] {EdgeVisible}");

                ValidateArgumentsRead("EdgeVisibilityCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeVisibilityCommand Error] {ex.Message}");
                EdgeVisible = DEFAULT_EDGE_VISIBLE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_VISIBILITY : {EdgeVisible}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
