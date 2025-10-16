using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class EdgeWidthSpecificationModeCommand : BaseCgmCommand
    {
        public SpecificationMode Mode { get; private set; }

        public EdgeWidthSpecificationModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EdgeWidthSpecificationModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                int mode = argReader.MakeEnum();
                Mode = SpecificationModeExtensions.GetMode(mode);
                CgmContext.EdgeWidthSpecificationMode = Mode;

                Debug.WriteLine($"[EdgeWidthSpecificationModeCommand] Mode={Mode}");
                ValidateArgumentsRead("EdgeWidthSpecificationModeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EdgeWidthSpecificationModeCommand ERROR] {ex.Message}");
                Mode = SpecificationMode.ABSOLUTE; // Valeur par défaut
                CgmContext.EdgeWidthSpecificationMode = Mode;
                HasReadErrors = true;
            }
        }
        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"EDGE_WIDTH_SPECIFICATION_MODE {Mode}";
        }
        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
