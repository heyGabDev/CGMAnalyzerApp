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
    public class InteriorStyleSpecificationModeCommand : BaseCgmCommand
    {
        public SpecificationMode Mode { get; private set; }
        private const SpecificationMode DEFAULT_MODE = SpecificationMode.ABSOLUTE;

        public InteriorStyleSpecificationModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[InteriorStyleSpecificationModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int mode = argReader.MakeEnum();
                Mode = SpecificationModeExtensions.GetMode(mode);
                CgmContext.InteriorStyleSpecificationMode = Mode;

                Debug.WriteLine($"[InteriorStyleSpecificationModeCommand] Mode={Mode}");
                ValidateArgumentsRead("InteriorStyleSpecificationModeCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[InteriorStyleSpecificationModeCommand ERROR] {ex.Message}");
                Mode = DEFAULT_MODE;
                CgmContext.InteriorStyleSpecificationMode = Mode;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"InteriorStyleSpecificationMode mode={Mode}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
