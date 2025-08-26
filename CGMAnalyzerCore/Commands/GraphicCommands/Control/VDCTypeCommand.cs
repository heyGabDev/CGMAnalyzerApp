using CGMAnalyzerCore.Context;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCTypeCommand : CgmCommand
    {
        
        public static VDCTypeEnum CurrentVDCType { get; private set; } = VDCTypeEnum.Integer;

        public VDCTypeEnum Type { get; }

        public VDCTypeCommand(int ec, int eid, int length, BinaryReader reader) : base(ec, eid, length, reader)
        {
            int vdcTypeCode = reader.ReadByte() & 0x1F;
            Type = vdcTypeCode switch
            {
                0 => VDCTypeEnum.Integer,
                1 => VDCTypeEnum.Real,
                _ => throw new InvalidDataException($"Unknown VDC type: {vdcTypeCode}")
            };

            CurrentVDCType = Type;
        }

        public override string ToString()
        {
            return $"VDCTypeCommand: {Type}";
        }

        public static void Reset()
        {
            CurrentVDCType = VDCTypeEnum.Integer;
        }

        public static int SizeOfVdc()
        {
            if (CurrentVDCType == VDCTypeEnum.Integer)
            {
                int precision = CgmContext.VdcIntegerPrecision; // VDCIntegerPrecision.getPrecision();
                return (precision / 8);
            }

            if (CurrentVDCType == VDCTypeEnum.Real)
            {
                Type precisionType = CgmContext.VdcRealPrecision.GetType();
                if (precisionType.Equals(VDCRealPrecisionEnum.FixedPoint32))
                {
                    return SizeOfFixedPoint32();
                }
                if (precisionType.Equals(VDCRealPrecisionEnum.FixedPoint32))
                {
                    return SizeOfFixedPoint64();
                }
                if (precisionType.Equals(VDCRealPrecisionEnum.FloatingPoint32))
                {
                    return SizeOfFloatingPoint32();
                }
                if (precisionType.Equals(VDCRealPrecisionEnum.FloatingPoint64))
                {
                    return SizeOfFloatingPoint64();
                }
            }
            return 1;
        }

        private static int SizeOfFixedPoint32()
        {
            return 2 + 2;
        }

        private static int SizeOfFixedPoint64()
        {
            return 4 + 4;
        }

        private static int SizeOfFloatingPoint32()
        {
            return 2 * 2;
        }

        private static int SizeOfFloatingPoint64()
        {
            return 2 * 4;
        }


    }
}
