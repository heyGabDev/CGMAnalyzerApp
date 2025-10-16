using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Parser;
using System.Diagnostics;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCTypeCommand : BaseCgmCommand
    {
        public static VDCTypeEnum CurrentVDCType { get; private set; } = VDCTypeEnum.Integer;

        public VDCTypeEnum Type { get; }

        public VDCTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                int vdcTypeCode = argReader.NextArg();

                Type = vdcTypeCode switch
                {
                    0 => VDCTypeEnum.Integer,
                    1 => VDCTypeEnum.Real,
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
                Type = VDCTypeEnum.Integer; // Valeur par défaut en cas d'erreur
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
