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
    public class MarkerSizeSpecificationModeCommand : BaseCgmCommand
    {
        public SpecificationMode Mode { get; private set; }

        public MarkerSizeSpecificationModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MarkerSizeSpecificationModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                int mode = argReader.MakeEnum();
                Mode = SpecificationModeExtensions.GetMode(mode);
                CgmContext.MarkerSizeSpecificationMode = Mode;

                Debug.WriteLine($"[MarkerSizeSpecificationModeCommand] Mode={Mode}");
                ValidateArgumentsRead("MarkerSizeSpecificationModeCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MarkerSizeSpecificationModeCommand ERROR] {ex.Message}");
                Mode = SpecificationMode.ABSOLUTE; // Valeur par défaut
                CgmContext.MarkerSizeSpecificationMode = Mode;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MARKER_SIZE_SPECIFICATION_MODE : {Mode}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
