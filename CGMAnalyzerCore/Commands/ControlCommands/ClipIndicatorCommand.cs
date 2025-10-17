using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ControlCommands
{
    public class ClipIndicatorCommand : BaseCgmCommand
    {
        public bool ClipFlag { get; private set; }
        private const bool DEFAULT_CLIP_FLAG = false;

        public ClipIndicatorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ClipIndicatorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
            var argReader = new ExtractedArgumentReader(this);
            ClipFlag = argReader.MakeEnum() == 1;
            Debug.WriteLine($"[ClipIndicatorCommand] ClipFlag={ClipFlag}");
                ValidateArgumentsRead("ClipIndicator");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ClipIndicatorCommand ERROR] {ex.Message}");
                ClipFlag = DEFAULT_CLIP_FLAG;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CLIP_INDICATOR : {ClipFlag}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
