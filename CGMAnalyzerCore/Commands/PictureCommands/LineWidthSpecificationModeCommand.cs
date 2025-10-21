using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.PictureCommands.MarkerSizeSpecificationModeCommand;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class LineWidthSpecificationModeCommand : BaseCgmCommand
    {
        public SpecificationMode Mode { get; private set; }
        private const SpecificationMode DEFAULT_MODE = SpecificationMode.ABSOLUTE;

        public LineWidthSpecificationModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[LineWidthSpecificationModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int mode = argReader.MakeEnum();
                Mode = SpecificationModeExtensions.GetMode(mode);
                CgmContext.LineWidthSpecificationMode = Mode;

                Debug.WriteLine($"[LineWidthSpecificationModeCommand] Mode={Mode}");
                ValidateArgumentsRead("LineWidthSpecificationModeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LineWidthSpecificationModeCommand ERROR] {ex.Message}");
                Mode = DEFAULT_MODE;
                CgmContext.LineWidthSpecificationMode = Mode;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"LINE_WIDTH_SPECIFICATION_MODE : {Mode}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
