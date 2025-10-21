using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Context;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    /// <summary>
    /// VDC_INTEGER_PRECISION (case 3, 1) - Définit la précision des coordonnées VDC entières
    /// </summary>
    public class VDCIntegerPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; } = 16;
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
                Debug.WriteLine($"[VDCIntegerPrecisionCommand] Precision={Precision} bits");

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
