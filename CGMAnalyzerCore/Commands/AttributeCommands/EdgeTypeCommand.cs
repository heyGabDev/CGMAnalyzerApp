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
    /// EDGE_TYPE (case 27) - Définit 
    /// </summary>
    public class EdgeTypeCommand : BaseCgmCommand
    {
        public int EdgeType { get; private set; } = 1;
        private const int DEFAULT_EDGE_TYPE = 1;

        public EdgeTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                EdgeType = argReader.MakeIndex();
                Debug.WriteLine($"[EdgeTypeCommand] EdgeType : {EdgeType}");

                ValidateArgumentsRead("EdgeTypeCommand");
            }
            catch (Exception ex )
            {
                Debug.WriteLine($"[EdgeTypeCommand Error] {ex.Message}");
                EdgeType = DEFAULT_EDGE_TYPE;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_TYPE : {EdgeType}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
