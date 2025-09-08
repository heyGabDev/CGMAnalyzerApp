using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCRealPrecisionCommand : CgmCommand
    {
        public VDCRealPrecisionEnum Precision { get; private set; }

        public VDCRealPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int p1 = argReader.MakeEnum();
            int p2 = argReader.MakeInt();
            int p3 = argReader.MakeInt();

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

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in VDCRealPrecision");
        }

        public static void Reset()
        {
            CgmContext.VdcRealPrecision = VDCRealPrecisionEnum.FixedPoint32;
        }

        public override string ToString()
        {
            return $"VDCRealPrecision {Precision}";
        }
    }
}
