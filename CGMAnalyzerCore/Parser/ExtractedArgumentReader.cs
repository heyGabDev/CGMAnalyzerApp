using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.GraphicCommands.Control;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Colors;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public string ReadString()
        {
            if (_command.AllArgumentsRead)
                return string.Empty;

            // 1. Lire d'abord la longueur de la chaîne (1 octet)
            int length = MakeUInt8();

            if (length < 0 || length > 255 || length == 0)  // CGM limite généralement à 255
                return string.Empty;

            int requiredArgs = length + ((length + 1) % 2);  // +padding si nécessaire
            if (_command.CurrentArg + requiredArgs > _command.Args.Length)
            {
                Debug.WriteLine($"[CGM] Pas assez d'arguments pour lire la chaîne (besoin: {requiredArgs}, disponible: {_command.Args.Length - _command.CurrentArg})");
                return string.Empty;
            }

            // 2. Lire les caractères un par un
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (_command.CurrentArg >= _command.Args.Length)
                {
                    // Protection contre lecture au-delà du tableau
                    break;
                }

                char c = (char)NextArg();
                sb.Append(c);
            }

            // 3. Gestion du padding CGM (alignement sur frontière paire)
            // Si la chaîne a une longueur impaire, sauter l'octet de padding
            if ((length + 1) % 2 == 1)
            {
                NextArg(); // Skip padding byte
            }

            return sb.ToString();
        }

        public int NextArg()
        {
            return _command.NextArg();
        }

        public void SkipBits()
        {
            _command.SkipBits();
        }

        #region MÉTHODES BASIQUES (basées sur makeUInt*)
        public int MakeUInt8()
        {
            SkipBits();
            return NextArg() & 0xFF;
        }

        public int MakeUInt16()
        {
            SkipBits();

            if (_command.CurrentArg + 1 < _command.Args.Length)
            {
                return (NextArg() << 8) | NextArg();
            }
            else if (_command.CurrentArg < _command.Args.Length)
            {
                // Fallback comme dans le Java original
                return NextArg();
            }

            return 0; // Comme assert false dans Java
        }

        public int MakeUInt24()
        {
            SkipBits();
            return (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }

        public int MakeUInt32()
        {
            SkipBits();
            return (NextArg() << 24) | (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }

        public int MakeUInt(int precision)
        {
            return precision switch
            {
                1 => MakeUInt1(),
                2 => MakeUInt2(),
                4 => MakeUInt4(),
                8 => MakeUInt8(),
                16 => MakeUInt16(),
                24 => MakeUInt24(),
                32 => MakeUInt32(),
                _ => MakeUInt8() // default comme dans Java
            };
        }
        #endregion

        #region MÉTHODES SIGNÉES (basées sur makeSignedInt*)
        public int MakeSignedInt8()
        {
            SkipBits();
            return (sbyte)NextArg();
        }

        public int MakeSignedInt16()
        {
            SkipBits();
            return (short)((NextArg() << 8) | NextArg());
        }

        public int MakeSignedInt24()
        {
            SkipBits();
            return (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }

        public int MakeSignedInt32()
        {
            SkipBits();
            return (NextArg() << 24) | (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }
        #endregion

        #region MÉTHODES SPÉCIALISÉES (basées sur make*) 
        public byte MakeByte()
        {
            SkipBits();
            return (byte)NextArg();
        }

        public char MakeChar()
        {
            SkipBits();
            return (char)NextArg();
        }

        public int MakeInt()
        {
            int precision = CgmContext.IntegerPrecision; // Obtenir depuis le contexte
            return MakeInt(precision);
        }

        public int MakeInt(int precision)
        {
            SkipBits();
            return precision switch
            {
                8 => MakeSignedInt8(),
                16 => MakeSignedInt16(),
                24 => MakeSignedInt24(),
                32 => MakeSignedInt32(),
                _ => MakeSignedInt16() // default
            };
        }

        public int MakeIndex()
        {
            int precision = CgmContext.IndexPrecision;
            return MakeInt(precision);
        }

        public int MakeName()
        {
            int precision = CgmContext.NamePrecision;
            return MakeInt(precision);
        }

        public int MakeEnum()
        {
            return MakeSignedInt16();
        }
        #endregion

        #region MÉTHODES POINTS ET VDC 
        public Point2D.Double MakePoint()
        {
            //TEST
            // Dépend de VDCType et VDCPrecision
            if (CgmContext.VdcType == VDCTypeEnum.Integer)
            {
                if (CgmContext.VdcIntegerPrecision == 16)
                {
                    var x = MakeSignedInt16();
                    var y = MakeSignedInt16();
                    return new Point2D.Double(x, y);
                }
                else if (CgmContext.VdcIntegerPrecision == 32)
                {
                    var x = MakeSignedInt32();
                    var y = MakeSignedInt32();
                    return new Point2D.Double(x, y);
                }
            }
            else // VDC Real
            {
                var x = MakeReal();
                var y = MakeReal();
                return new Point2D.Double(x, y);
            }

            throw new NotSupportedException($"VDC precision non supportée");

            //return new Point2D.Double(MakeVdc(), MakeVdc());
        }

        public Point2D.Double MakePoint(int ec, int eid)
        {
            return new Point2D.Double(MakeVdc(), MakeVdc());
        }

        public double MakeVdc()
        {
            if (CgmContext.VdcType == VDCTypeEnum.Real)
            {
                var precision = CgmContext.VdcRealPrecision;
                return precision switch
                {
                    VDCRealPrecisionEnum.FixedPoint32 => MakeFixedPoint32(),
                    VDCRealPrecisionEnum.FixedPoint64 => MakeFixedPoint64(),
                    VDCRealPrecisionEnum.FloatingPoint32 => MakeFloatingPoint32(),
                    VDCRealPrecisionEnum.FloatingPoint64 => MakeFloatingPoint64(),
                    _ => MakeFixedPoint32()
                };
            }

            // Integer VDC
            int intPrecision = CgmContext.VdcIntegerPrecision;
            return intPrecision switch
            {
                16 => MakeSignedInt16(),
                24 => MakeSignedInt24(),
                32 => MakeSignedInt32(),
                _ => MakeSignedInt16()
            };
        }

        public double MakeVc()
        {
            // Device viewport coordinates - based on specification mode
            return MakeReal(); // Simplified
        }
        #endregion

        #region MÉTHODES NOMBRES RÉELS
        public double MakeReal()
        {
            var precision = CgmContext.RealPrecision;
            return precision switch
            {
                0 => MakeFixedPoint32(),      // Fixed32
                1 => MakeFixedPoint64(),      // Fixed64
                2 => MakeFloatingPoint32(),   // Floating32
                3 => MakeFloatingPoint64(),   // Floating64
                _ => MakeFixedPoint32()       // Default
            };
        }

        public double MakeFixedPoint32()
        {
            double wholePart = MakeSignedInt16();
            double fractionPart = MakeUInt16();
            return wholePart + (fractionPart / (1 << 16)); // Correction du calcul
        }

        public double MakeFixedPoint64()
        {
            double wholePart = MakeSignedInt32();
            double fractionPart = MakeUInt32();
            return wholePart + (fractionPart / (1L << 32));
        }

        public double MakeFloatingPoint()
        {
            var precision = CgmContext.RealPrecision;
            if (precision == 2) // FLOATING_32
            {
                return MakeFloatingPoint32();
            }
            if (precision == 3) // FLOATING_64
            {
                return MakeFloatingPoint64();
            }
            return MakeFloatingPoint32();
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

        public int MakeInt16()
        {
            return MakeSignedInt16();
        }

        public int MakeInt32()
        {
            return MakeSignedInt32();
        }

        // <summary>
        /// Lit un float 32 bits
        /// </summary>
        public float MakeFloat32()
        {
            int bits = MakeSignedInt32();
            return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0);
        }
        #endregion

        #region MÉTHODES COULEURS 
        public object MakeColorValue()
        {
            // Pour les composants de couleur individuels
            int precision = CgmContext.ColorPrecision;
            return MakeUInt(precision);
        }

        public int MakeColorIndex()
        {
            int precision = CgmContext.ColorIndexPrecision;
            return MakeUInt(precision);
        }

        public int MakeColorIndex(int precision)
        {
            return MakeUInt(precision);
        }

        public System.Drawing.Color MakeDirectColor()
        {
            int precision = CgmContext.ColorPrecision;
            var model = CgmContext.ColourModel;

            if (model == ColorModelEnum.RGB)
            {
                int r = ScaleColorValueRGB(MakeUInt(precision));
                int g = ScaleColorValueRGB(MakeUInt(precision));
                int b = ScaleColorValueRGB(MakeUInt(precision));
                return System.Drawing.Color.FromArgb(r, g, b);
            }

            // Autres modèles de couleur non implémentés
            MakeUInt(precision); // Consommer les arguments
            MakeUInt(precision);
            MakeUInt(precision);
            return System.Drawing.Color.Cyan;
        }

        private int ScaleColorValueRGB(int value)
        {
            // Simplification - scaling basé sur les extents de couleur
            var min = CgmContext.MinimumColorValueRGB;
            var max = CgmContext.MaximumColorValueRGB;

            if (min != null && max != null && max[0] != min[0])
            {
                return 255 * (value - min[0]) / (max[0] - min[0]);
            }

            return Math.Min(255, Math.Max(0, value)); // Clamp par défaut
        }
        #endregion

        #region MÉTHODES CHAÎNES 
        public string MakeString()
        {
            // TEST
            int length = MakeByte();
            if (length == 255)
            {
                length = MakeUInt16();
            }

            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = MakeByte();
            }

            return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytes);

            //int length = GetStringCount();
            //byte[] bytes = new byte[length];
            //for (int i = 0; i < length; i++)
            //{
            //    bytes[i] = MakeByte();
            //}

            //try
            //{
            //    return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            //}
            //catch
            //{
            //    return Encoding.Default.GetString(bytes);
            //}
        }

        public string MakeFixedString()
        {
            int length = GetStringCount();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = MakeChar();
            }
            return new string(chars);
        }

        private int GetStringCount()
        {
            int length = MakeUInt8();
            if (length == 255)
            {
                length = MakeUInt16();
                if ((length & (1 << 15)) != 0) // bit 15 set
                {
                    length = (length << 16) | MakeUInt16();
                }
            }
            return length;
        }
        #endregion

        #region MÉTHODES DE TAILLE
        public int SizeOfPoint()
        {
            return 2 * SizeOfVdc();
        }

        public int SizeOfVdc()
        {
            if (CgmContext.VdcType == VDCTypeEnum.Integer)
            {
                return CgmContext.VdcIntegerPrecision / 8;
            }

            return CgmContext.VdcRealPrecision switch
            {
                VDCRealPrecisionEnum.FixedPoint32 => 4,
                VDCRealPrecisionEnum.FixedPoint64 => 8,
                VDCRealPrecisionEnum.FloatingPoint32 => 4,
                VDCRealPrecisionEnum.FloatingPoint64 => 8,
                _ => 2
            };
        }

        public int SizeOfInt()
        {
            return CgmContext.IntegerPrecision / 8;
        }

        public int SizeOfIndex()
        {
            return CgmContext.IndexPrecision / 8;
        }

        public int SizeOfEnum()
        {
            return 2; // Standard CGM
        }
        #endregion

        #region MÉTHODES BIT

        private int MakeUInt1()
        {
            return MakeUIntBit(1);
        }

        private int MakeUInt2()
        {
            return MakeUIntBit(2);
        }

        private int MakeUInt4()
        {
            return MakeUIntBit(4);
        }

        private int MakeUIntBit(int numBits)
        {
            if (_command.CurrentArg >= _command.Args.Length)
                return 0;

            int bitsPosition = 8 - numBits - _command.PosInArg;
            int mask = ((1 << numBits) - 1) << bitsPosition;
            int ret = (_command.Args[_command.CurrentArg] & mask) >> bitsPosition;

            _command.PosInArg += numBits;
            if (_command.PosInArg % 8 == 0)
            {
                _command.PosInArg = 0;
                _command.CurrentArg++;
            }

            return ret;
        }

        #endregion

        #region MÉTHODES NON IMPLÉMENTÉES (placeholder)
        public object MakeBitStream()
        {
            // Implémentation complexe - placeholder
            return new byte[0];
        }

        public object MakeColorList()
        {
            // Liste de couleurs - implémentation complexe
            return new List<System.Drawing.Color>();
        }
        #endregion
    }
}
