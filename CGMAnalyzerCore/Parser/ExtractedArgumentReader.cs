using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.GraphicCommands.Control;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Geometry.Point2D;

namespace CGMAnalyzerCore.Parser
{
    /// <summary>
    /// Lit des arguments CGM à partir d'un tableau d'octets déjà extraits (args[]).
    /// Utilisé après que la commande ait été entièrement lue depuis le flux binaire.
    /// </summary>
    public class ExtractedArgumentReader
    {
        private readonly CgmCommand _command;


        public ExtractedArgumentReader(CgmCommand command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public int NextArg()
        {
            return _command.NextArg();
        }

        public void SkipBits()
        {
            _command.SkipBits();
        }

        public int MakeUInt8()
        {
            return NextArg() & 0xFF;
        }

        public string MakeString()
        {
            int length = GetStringCount();
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)(NextArg() & 0xFF);
            }

            try
            {
                return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            }
            catch
            {
                return Encoding.Default.GetString(bytes);
            }
        }

        public float MakeFloatCoord()
        {
            return NextArg();
        }

        public short MakeSignedInt16()
        {

            return unchecked((short)(NextArg() & 0xFFFF));
        }

        public int MakeSignedInt24()
        {
            int b1 = NextArg() & 0xFF;
            int b2 = NextArg() & 0xFF;
            int b3 = NextArg() & 0xFF;
            int value = (b1 << 16) | (b2 << 8) | b3;
            if ((value & 0x800000) != 0)
                value |= unchecked((int)0xFF000000); // sign extend
            return value;
        }

        public int MakeSignedInt32()
        {
            int high = NextArg();
            int low = NextArg();
            return (high << 16) | low;
        }

        public int MakeInt()
        {
            int precision = CgmContext.VdcIntegerPrecision;
            return MakeInt(precision);
        }

        public int MakeInt(int precision)
        {
            return precision switch
            {
                8 => (sbyte)NextArg(),
                16 => (short)((NextArg() << 8) | NextArg()),
                24 => (NextArg() << 16) | (NextArg() << 8) | NextArg(),
                32 => (NextArg() << 24) | (NextArg() << 16) | (NextArg() << 8) | NextArg(),
                _ => throw new NotSupportedException($"Unsupported integer precision: {precision}")
            };
        }

        public Point2D.Double MakePoint(int ec, int eid)
        {
            return new Point2D.Double(MakeVdc(ec, eid), MakeVdc(ec, eid));
        }

        public double MakeVdc(int ec, int eid)
        {
            if (VDCTypeCommand.CurrentVDCType == VDCTypeEnum.Real)
            {
                var precision = CgmContext.VdcRealPrecision;
                switch (precision)
                {
                    case VDCRealPrecisionEnum.FixedPoint32:
                        //VDCRealPrecision.Type.FixedPoint32Bit:
                        return MakeFixedPoint32();
                    case VDCRealPrecisionEnum.FixedPoint64:
                        return MakeFixedPoint64();
                    case VDCRealPrecisionEnum.FloatingPoint32:
                    //return MakeFloatingPoint32();
                    case VDCRealPrecisionEnum.FloatingPoint64:
                    //return MakeFloatingPoint64();
                    default:
                        UnsupportedCommand.Unsupported(ec, eid, $"unsupported precision {precision}");
                        return MakeFixedPoint32();
                }
            }

            // Assume integer if not real
            int intPrecision = CgmContext.VdcIntegerPrecision;
            return intPrecision switch
            {
                16 => MakeSignedInt16(),
                24 => MakeSignedInt24(),
                32 => MakeSignedInt32(),
                _ => throw new NotSupportedException($"Unsupported integer VdcIntegerPrecision: {intPrecision}")
            };
        }

        public int SizeOfPoint()
        {
            return 2 * VDCTypeCommand.SizeOfVdc();
        }

        public static double MakeFixedPoint32()
        {
            double wholePart = 1; //makeSignedInt16();
            double fractionPart = 1;//makeUInt16();

            return wholePart + (fractionPart / (2 << 15));
        }

        public static double MakeFixedPoint64()
        {
            double wholePart = 2; //makeSignedInt32();
            double fractionPart = 2; //makeUInt32();

            return wholePart + (fractionPart / (2 << 31));
        }

        public double MakeFloatingPoint32()
        {
            SkipBits();
            int bits = 0;
            for (int i = 0; i < 4; i++)
            {
                bits = (bits << 8) | MakeChar();
            }
            return BitConverter.Int32BitsToSingle(bits);
        }

        public double MakeFloatingPoint64()
        {
            SkipBits();
            long bits = 0;
            for (int i = 0; i < 8; i++)
            {
                bits = (bits << 8) | MakeChar();
            }
            return BitConverter.Int64BitsToDouble(bits);
        }

        public char MakeChar()
        {
            _command.SkipBits();

            if (_command.CurrentArg >= _command.Args.Length)
                throw new IndexOutOfRangeException("Attempted to read beyond the end of the argument list.");

            return (char)(_command.Args[_command.CurrentArg++]);
        }

        //public string MakeString(BinaryReader reader)
        //{
        //    int length = GetStringCount(reader);
        //    byte[] bytes = new byte[length];

        //    for (int i = 0; i < length; i++)
        //    {
        //        bytes[i] = MakeByte(reader);
        //    }

        //    try
        //    {
        //        return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
        //    }
        //    catch
        //    {
        //        return System.Text.Encoding.Default.GetString(bytes);
        //    }
        //}

        public int GetStringCount()
        {
            int length = MakeUInt8();
            if (length == 255)
            {
                length = MakeUInt16();
                if ((length & (1 << 16)) != 0)
                {
                    length = (length << 16) | MakeUInt16();
                }
            }
            return length;
        }

        public byte MakeByte(BinaryReader reader)
        {
            SkipBits();
            return reader.ReadByte();
        }

        public int MakeUInt8(BinaryReader reader)
        {
            SkipBits(); // Appelle ta méthode existante
            return reader.ReadByte(); // Byte = 8 bits non signé
        }

        public int MakeUInt16(BinaryReader reader)
        {
            SkipBits(); // Appelle ta méthode existante

            // On vérifie s’il reste 2 octets à lire
            if (reader.BaseStream.Length - reader.BaseStream.Position >= 2)
            {
                byte high = reader.ReadByte();
                byte low = reader.ReadByte();
                return (high << 8) | low;
            }

            // Si seulement 1 octet reste
            if (reader.BaseStream.Length - reader.BaseStream.Position >= 1)
            {
                // Optionnel : log de fallback
                return reader.ReadByte();
            }

            throw new EndOfStreamException("Unexpected end of stream when trying to read UInt16.");
        }

        public double MakeVdc()
        {
            SkipBits();

            if (CgmContext.VdcType == VDCTypeEnum.Real)
            {
                var precision = CgmContext.VdcRealPrecision;
                return precision switch
                {
                    VDCRealPrecisionEnum.FixedPoint32 => MakeFixedPoint32(),
                    VDCRealPrecisionEnum.FixedPoint64 => MakeFixedPoint64(),
                    VDCRealPrecisionEnum.FloatingPoint32 => MakeFloatingPoint32(),
                    VDCRealPrecisionEnum.FloatingPoint64 => MakeFloatingPoint64(),
                    _ => throw new NotSupportedException($"Unsupported real VDC precision: {precision}")
                };
            }

            int intPrecision = CgmContext.VdcIntegerPrecision;
            return intPrecision switch
            {
                16 => MakeSignedInt16(),
                24 => MakeSignedInt24(),
                32 => MakeSignedInt32(),
                _ => throw new NotSupportedException($"Unsupported integer VDC precision: {intPrecision}")
            };
        }

        public short MakeEnum()
        {
            return MakeSignedInt16();
        }

        public double MakeReal()
        {
            var precision = CgmContext.VdcRealPrecision;

            if (precision.Equals(VDCRealPrecisionEnum.FixedPoint32))
            {
                return MakeFixedPoint32();
            }
            if (precision.Equals(VDCRealPrecisionEnum.FixedPoint64))
            {
                return MakeFixedPoint64();
            }
            if (precision.Equals(VDCRealPrecisionEnum.FloatingPoint32))
            {
                return MakeFloatingPoint32();
            }
            if (precision.Equals(VDCRealPrecisionEnum.FloatingPoint64))
            {
                return MakeFloatingPoint64();
            }

            // unsupported("unsupported real precision "+precision);
            return MakeFloatingPoint32();
        }
   
        pub
    }
}
