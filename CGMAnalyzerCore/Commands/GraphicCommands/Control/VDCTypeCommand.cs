using CGMAnalyzerCore.Context; // Ajouté pour VDCTypeEnum
using CGMAnalyzerCore.Parser;
using System.Diagnostics;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCTypeCommand : BaseCgmCommand
    {
        public enum VDCTypeEnum
        {
            INTEGER = 0,
            REAL = 1
        }

        public static VDCTypeEnum CurrentVDCType { get; private set; }
        private const VDCTypeEnum DEFAULT_VDC_TYPE = VDCTypeEnum.INTEGER;

        public VDCTypeEnum Type { get; } = VDCTypeEnum.INTEGER;// Valeur par défaut en cas d'erreur

        public VDCTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int vdcTypeCode = argReader.NextArg();

                Type = vdcTypeCode switch
                {
                    0 => VDCTypeEnum.INTEGER,
                    1 => VDCTypeEnum.REAL,
                    _ => throw new InvalidDataException($"Unknown VDC type: {vdcTypeCode}")
                };

                // Mettre à jour le contexte CGM  & le contexte global
                CurrentVDCType = Type;
                CgmContext.SetVdcType(Type);

                Debug.WriteLine($"[VDCTypeCommand] Type={Type}");
                ValidateArgumentsRead("VDCTypeCommand");
            }
            catch (Exception)
            {
                Debug.WriteLine($"[VDCTypeCommand ERROR] Error reading VDCTypeCommand");
                Type = DEFAULT_VDC_TYPE; 
                CurrentVDCType = Type;
                CgmContext.SetVdcType(Type);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"VDC_TYPE : {Type}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }

        //public static void Reset()
        //{
        //    CurrentVDCType = VDCTypeEnum.Integer;
        //}

        //public static int SizeOfVdc()
        //{
        //    if (CurrentVDCType == VDCTypeEnum.Integer)
        //    {
        //        int precision = CgmContext.VdcIntegerPrecision; // VDCIntegerPrecision.getPrecision();
        //        return (precision / 8);
        //    }

        //    if (CurrentVDCType == VDCTypeEnum.Real)
        //    {
        //        Type precisionType = CgmContext.VdcRealPrecision.GetType();
        //        if (precisionType.Equals(VDCRealPrecisionEnum.FixedPoint32))
        //        {
        //            return SizeOfFixedPoint32();
        //        }
        //        if (precisionType.Equals(VDCRealPrecisionEnum.FixedPoint32))
        //        {
        //            return SizeOfFixedPoint64();
        //        }
        //        if (precisionType.Equals(VDCRealPrecisionEnum.FloatingPoint32))
        //        {
        //            return SizeOfFloatingPoint32();
        //        }
        //        if (precisionType.Equals(VDCRealPrecisionEnum.FloatingPoint64))
        //        {
        //            return SizeOfFloatingPoint64();
        //        }
        //    }
        //    return 1;
        //}

        //private static int SizeOfFixedPoint32()
        //{
        //    return 2 + 2;
        //}

        //private static int SizeOfFixedPoint64()
        //{
        //    return 4 + 4;
        //}

        //private static int SizeOfFloatingPoint32()
        //{
        //    return 2 * 2;
        //}

        //private static int SizeOfFloatingPoint64()
        //{
        //    return 2 * 4;
        //}
    }
}
