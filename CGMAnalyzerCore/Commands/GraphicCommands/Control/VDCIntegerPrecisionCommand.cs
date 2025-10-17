using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCIntegerPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; }
        private const int DEFAULT_PRECISION = 16;

        public VDCIntegerPrecisionCommand(int ec, int eid, int l, CgmCommand command)
        : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCIntegerPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);            
                Precision = argReader.MakeInt();
                CgmContext.VdcIntegerPrecision = Precision;
                Debug.WriteLine($"[VDCIntegerPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("VDCIntegerPrecisionCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VDCIntegerPrecisionCommand ERROR] {ex.Message}");
                Precision = DEFAULT_PRECISION;
                CgmContext.VdcIntegerPrecision = Precision;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"VDC_INTEGER_PRECISION : {Precision} bits";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
