using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MaximumColorIndexCommand : BaseCgmCommand
    {
        public int MaxColorIndex { get; private set; }
        private const int DEFAULT_MAX_COLOR_INDEX = 255;

        public MaximumColorIndexCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MaximumColorIndexCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                MaxColorIndex = argReader.MakeColorIndex();
                CgmContext.MaximumColorIndex = MaxColorIndex;
                Debug.WriteLine($"[MaximumColorIndexCommand] MaxColorIndex={MaxColorIndex}");
                ValidateArgumentsRead("MaximumColorIndexCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaximumColorIndexCommand ERROR]  {ex.Message}");
                MaxColorIndex = DEFAULT_MAX_COLOR_INDEX;
                CgmContext.MaximumColorIndex = MaxColorIndex;
                HasReadErrors = true;
            }
        }

        public override void Draw(System.Drawing.Graphics g, System.Drawing.Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MAXIMUM_COLOR_INDEX : {MaxColorIndex}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
