using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCRealPrecisionCommand : BaseCgmCommand
    {
        public VDCRealPrecisionEnum Precision { get; private set; }
        private const VDCRealPrecisionEnum DEFAULT_PRECISION = VDCRealPrecisionEnum.FixedPoint32;

        public VDCRealPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCRealPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int p1 = argReader.MakeEnum();
                int p2 = argReader.MakeInt();
                int p3 = argReader.MakeInt();
                Debug.WriteLine($"[VDCRealPrecisionCommand] Read values: p1={p1}, p2={p2}, p3={p3}");

                if (p1 == 0) // Floating point
                {
                    if (p2 == 9 && p3 == 23)
                    {
                        Precision = VDCRealPrecisionEnum.FloatingPoint32;
                    }
                    else if (p2 == 12 && p3 == 52)
                    {
                        Precision = VDCRealPrecisionEnum.FloatingPoint64;
                    }
                    else
                    {
                        // Use default
                        Precision = VDCRealPrecisionEnum.FixedPoint32;
                    }
                }
                else if (p1 == 1) // Fixed point
                {
                    if (p2 == 16 && p3 == 16)
                    {
                        Precision = VDCRealPrecisionEnum.FixedPoint32;
                    }
                    else if (p2 == 32 && p3 == 32)
                    {
                        Precision = VDCRealPrecisionEnum.FixedPoint64;
                    }
                    else
                    {
                        // Use default
                        Precision = VDCRealPrecisionEnum.FixedPoint32;
                    }
                }
                else
                {
                    Precision = VDCRealPrecisionEnum.FixedPoint32;
                }
                CgmContext.VdcRealPrecision = Precision;
                Debug.WriteLine($"[VDCRealPrecisionCommand] Set Precision={Precision}");
                ValidateArgumentsRead("VDCRealPrecisionCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VDCRealPrecisionCommand ERROR] {ex.Message}");
                Precision = DEFAULT_PRECISION;
                CgmContext.VdcRealPrecision = Precision;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"VDC_REAL_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
