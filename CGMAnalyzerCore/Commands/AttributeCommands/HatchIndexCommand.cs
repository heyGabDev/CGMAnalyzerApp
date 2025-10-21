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
    /// HATCH_INDEX (case 24) - Définit 
    /// </summary>
    public class HatchIndexCommand : BaseCgmCommand
    {
        public int HatchIndex { get; private set; } = 1;
        private const int DEFAULT_HATCH_INDEX = 1;

        public HatchIndexCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[HatchIndexCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                HatchIndex = argReader.MakeIndex();
                Debug.WriteLine($"[HatchIndexCommand] HatchIndex : {HatchIndex}");

                ValidateArgumentsRead("HatchIndex");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HatchIndexCommand Error] {ex.Message}");
                HatchIndex = DEFAULT_HATCH_INDEX;   
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"HATCH_INDEX : {HatchIndex}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
