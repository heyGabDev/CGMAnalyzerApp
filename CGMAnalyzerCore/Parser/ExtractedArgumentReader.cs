using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Geometry;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static CGMAnalyzerCore.Commands.GraphicCommands.Control.VDCTypeCommand;
using static CGMAnalyzerCore.Commands.MetafileCommands.ColorModelCommand;

namespace CGMAnalyzerCore.Parser
{
    /// <summary>
    /// Lit des arguments CGM à partir d'un tableau d'octets déjà extraits (args[]).
    /// Utilisé après que la commande ait été entièrement lue depuis le flux binaire.
    /// </summary>
    public class ExtractedArgumentReader
    {
        private readonly BaseCgmCommand _command;

        public ExtractedArgumentReader(BaseCgmCommand command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public int NextArg()
        {
            return _command.TryNextArg(out int value) ? value : 0;
        }

        public void SkipBits()
        {
            _command.SkipBits();
        }

        #region ===== LECTURE D'ENTIERS NON SIGNÉS =====
        //MÉTHODES BASIQUES (basées sur makeUInt*)

        /// <summary>
        /// Lit un octet (8 bits) / Validation incluse dans NextArg()
        /// </summary>
        /// <returns></returns>
        public byte MakeByte()
        {
            return (byte)NextArg();
        }

        // <summary>
        /// Lit un UInt8 non signé (1 octet)
        /// </summary>
        public int MakeUInt8()
        {
            SkipBits();

            if (!_command.ValidateRemainingArgs(1, "MakeUInt8"))
            {
                return 0;
            }

            return NextArg() & 0xFF;
        }

        /// <summary>
        /// Lit un UInt16 non signé (2 octets non signés)
        /// </summary>
        /// <returns></returns>
        public int MakeUInt16()
        {
            SkipBits();

            if (!_command.ValidateRemainingArgs(2, "MakeUInt16"))
            {
                // Fallback : si un seul octet disponible, le retourner
                if (_command.ValidateRemainingArgs(1, "MakeUInt16 fallback"))
                {
                    return NextArg();
                }

                return 0;
            }

            return (NextArg() << 8) | NextArg();
        }

        /// <summary>
        /// Lit un UInt24 non signé (3 octets non signés)
        /// </summary>
        /// <returns></returns>
        public int MakeUInt24()
        {
            SkipBits();
            if (!_command.ValidateRemainingArgs(3, "MakeUInt24"))
            {
                return 0;
            }

            return (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }

        /// <summary>
        /// Lit un UInt32 non signé (4 octets non signés)
        /// </summary>
        /// <returns></returns>
        public int MakeUInt32()
        {
            SkipBits();
            if (!_command.ValidateRemainingArgs(4, "MakeUInt32"))
            {
                return 0;
            }

            return (NextArg() << 24) | (NextArg() << 16) | (NextArg() << 8) | NextArg();
        }

        /// <summary>
        /// Lit un UInt avec précision variable (1, 2, 4, 8, 16, 24, 32 bits)
        /// </summary>
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

        #region ===== LECTURE D'ENTIERS SIGNÉS =====
        /// <summary>
        /// Lit un Int8 signé (1 octet)
        /// Pas de SkipBits() : appelé depuis MakeInt() qui le fait déjà
        /// </summary>
        /// <returns></returns>
        public int MakeSignedInt8()
        {
            if(!_command.ValidateRemainingArgs(1, "MakeSignedInt8"))
            {
                return 0;
            }
            int value = NextArg();

            // Étendre le signe
            if ((value & 0x80) != 0) // Si le bit de signe est défini
            {
                value |= unchecked((int)0xFFFFFF00); // Étendre le signe pour 32 bits
            }
            return value;
        }

        /// <summary>
        /// Lit un Int16 signé (2 octets)
        /// Pas de SkipBits() : appelé depuis MakeInt() qui le fait déjà
        /// </summary>
        /// <returns></returns>
        public int MakeSignedInt16()
        {
           if(!_command.ValidateRemainingArgs(2, "MakeSignedInt16"))
           {
                return 0;
           }

           int value = (NextArg() << 8) | NextArg();
           if((value & 0x8000) != 0) // Si le bit de signe est défini
           {
                value |= unchecked((int)0xFFFF0000); // Étendre le signe pour 32 bits
           }
           return value;
        }

        /// <summary>
        /// Lit un Int24 signé (3 octets)
        /// Pas de SkipBits() : appelé depuis MakeInt() qui le fait déjà
        /// </summary>
        /// <returns></returns>
        public int MakeSignedInt24()
        {
           if(!_command.ValidateRemainingArgs(3, "MakeSignedInt24"))
           {
                return 0;
           }

           int value = (NextArg() << 16) | (NextArg() << 8) | NextArg();

           if((value & 0x800000) != 0)
           {
                value |= unchecked((int)0xFF000000); 
           }
           return value;
        }

        /// <summary>
        /// Lit un Int32 signé (4 octets)
        /// Pas de SkipBits() : appelé depuis MakeInt() qui le fait déjà
        /// </summary>
        /// <returns></returns>
        public int MakeSignedInt32()
        {
           if(!_command.ValidateRemainingArgs(4, "MakeSignedInt32"))
           {
                return 0;
           }

           int value = (NextArg() << 24) | (NextArg() << 16) | (NextArg() << 8) | NextArg();
           return value;
        }
        #endregion

        #region ===== MÉTHODES SPÉCIALISÉES (basées sur make*) ===== 

        public char MakeChar()
        {
            SkipBits();
            return (char)NextArg();
        }

        /// <summary>
        /// Lit un entier avec la précision définie dans le contexte CGM.
        /// Usage commandes
        /// </summary>
        /// <returns></returns>
        public int MakeInt()
        {
            int precision = CgmContext.IntegerPrecision; // Obtenir depuis le contexte
            return MakeInt(precision);
        }

        /// <summary>
        /// Lit un entier avec la précision spécifiée (8, 16, 24, 32 bits).
        /// Usage interne au fichier
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        public int MakeInt(int precision)
        {
           int bytesNeeded = precision / 8;

            if (!_command.ValidateRemainingArgs(bytesNeeded,$"MakeInt({precision}")) 
            {
                return 0;
            }

            SkipBits();

            return precision switch
            {
                8 => MakeSignedInt8(),
                16 => MakeSignedInt16(),
                24 => MakeSignedInt24(),
                32 => MakeSignedInt32(),
                _ =>  0
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
        
        public int MakeInt16()
        {
            return MakeSignedInt16();
        }

        public int MakeInt32()
        {
            return MakeSignedInt32();
        }
        #endregion

        #region ===== MÉTHODES POINTS ET VDC ===== 
        /// <summary>
        /// Lit un Point2D simple (sans paramètres ec/eid)
        /// </summary>
        public Point2D.Double MakePoint()
        {
            int bytesNeeded = CalculatePointSize();

            if (!_command.ValidateRemainingArgs(bytesNeeded, "MakePoint"))
            {
                return new Point2D.Double(0, 0);
            }

            if(CgmContext.VdcType == VDCTypeEnum.INTERGER)
            {
                if(CgmContext.VdcIntegerPrecision == 16)
                {
                    var x = MakeSignedInt16();
                    var y = MakeSignedInt16();
                    return new Point2D.Double(x, y);
                }
                //else if(CgmContext.VdcIntegerPrecision == 24)
                //{
                //    var x = MakeSignedInt24();
                //    var y = MakeSignedInt24();
                //    return new Point2D.Double(x, y);
                //}
                else if(CgmContext.VdcIntegerPrecision == 32)
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

            throw new NotSupportedException($"VDC precision non supportée : {CgmContext.VdcIntegerPrecision}");
        }

        /// <summary>
        /// Lit un Point2D avec des arguments EC/EID pour le debug.
        /// </summary>
        /// <param name="ec"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        public Point2D.Double MakePoint(int ec, int eid)
        {
            int bytesNeeded = CalculatePointSize();
            if (!_command.ValidateRemainingArgs(bytesNeeded, $"MakePoint(ec={ec}, eid={eid})"))
            {
                return new Point2D.Double(0, 0);
            }
            return new Point2D.Double(MakeVdc(), MakeVdc());
        }

        /// <summary>
        /// Lit une coordonnée VDC ((Virtual Device Coordinate))
        /// (X ou Y) en fonction du type et de la précision définis dans le contexte CGM.
        /// </summary>
        /// <returns></returns>
        public double MakeVdc()
        {
            if (CgmContext.VdcType == VDCTypeEnum.REAL)
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

        /// <summary>
        /// Device viewport coordinates - basé sur le mode de spécification.
        /// </summary>
        /// <returns></returns>
        public double MakeVc()
        {
            return MakeReal();
        }

        /// <summary>
        /// Calcule la taille d'un point en octets
        /// </summary>
        private int CalculatePointSize()
        {
            return 2 * SizeOfVdc(); // 2 coordonnées × taille VDC
        }
        #endregion

        #region ===== MÉTHODES NOMBRES RÉELS ===== 
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
            if(!_command.ValidateRemainingArgs(4, "MakeFixedPoint32"))
            {
                return 0.0;
            }

            double wholePart = MakeSignedInt16();
            double fractionPart = MakeUInt16();
            return wholePart + (fractionPart / 65536.0); // 2^16
        }

        public double MakeFixedPoint64()
        {
            if(!_command.ValidateRemainingArgs(8, "MakeFixedPoint64"))
            {
                return 0.0;
            }

            double wholePart = MakeSignedInt32();
            double fractionPart = MakeUInt32();
            return wholePart + (fractionPart / 4294967296.0); // 2^32
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
            if(!_command.ValidateRemainingArgs(4, "MakeFloatingPoint32"))
            {
                return 0.0;
            }

            SkipBits();
            int bits = 0;
            for (int i = 0; i < 4; i++)
            {
                bits = (bits << 8) | NextArg();
            }
            return BitConverter.Int32BitsToSingle(bits);
        }
        
        public double MakeFloatingPoint64()
        {
            if(!_command.ValidateRemainingArgs(8, "MakeFloatingPoint64"))
            {
                return 0.0;
            }

            SkipBits();
            long bits = 0;
            for (int i = 0; i < 8; i++)
            {
                bits = (bits << 8) | NextArg();
            }
            return BitConverter.Int64BitsToDouble(bits);
        }

        /// <summary>
        /// Lit un float 32 bits
        /// </summary>
        public float MakeFloat32()
        {
            int bits = MakeSignedInt32();
            return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0);
        }
        #endregion

        #region ===== MÉTHODES COULEURS ===== 
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
            int bytesNeeded = (precision / 8) * 3;// RGB = 3 composants

            if (!_command.ValidateRemainingArgs(bytesNeeded, "MakeDirectColor"))
            {
                return System.Drawing.Color.Black; // Couleur par défaut en cas d'erreur
            }
            var model = CgmContext.ColorModel;

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

            return Math.Min(255, Math.Max(0, value));
        }
        #endregion

        #region ===== MÉTHODES CHAÎNES ===== 
        public string ReadString()
        {
            if (_command.AllArgumentsRead)
                return string.Empty;

            // 1. Lire d'abord la longueur de la chaîne (1 octet)
            int length = MakeUInt8();

            // Debug pour voir ce qui se passe
            Debug.WriteLine($"[ReadString] Length: {length}, CurrentArg: {_command.CurrentArg}/{_command.Args.Length}");


            if (length < 0 || length > 255)  // CGM limite généralement à 255
            {
                Debug.WriteLine($"[ReadString] Longueur invalide: {length}");
                return string.Empty;
            }

            // 2. Vérifier qu'il y a assez d'arguments restants
            if (_command.RemainingArgs() < length)
            {
                Debug.WriteLine($"[ReadString] Pas assez d'octets: besoin de {length}, reste {_command.Args.Length - _command.CurrentArg}");
                return string.Empty;
            }

            // 3. Lire les caractères
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (!_command.HasMoreArgs())
                {
                    Debug.WriteLine($"[ReadString] Fin prématurée à l'octet {i}/{length}");
                    break;
                }
                char c = (char)NextArg();
                sb.Append(c);
            }

            // 4.Gestion correcte du padding CGM
            // Le padding est nécessaire si (longueur + 1 octet de longueur) est impair
            // Car CGM aligne sur frontière de mot (2 octets)
            int totalBytesRead = 1 + length; // 1 pour la longueur + length pour la chaîne
            if (totalBytesRead % 2 == 1 && _command.HasMoreArgs())
            {
                Debug.WriteLine($"[ReadString] Skip padding byte");
                NextArg(); // Skip padding byte
            }

            string result = sb.ToString();
            Debug.WriteLine($"[ReadString] Lu: '{result}'");
            return result;
        }

        public string MakeString()
        {
            int length = MakeByte();
            if (length == 255)
            {
                length = MakeUInt16();
            }

            if(!_command.ValidateRemainingArgs(length, "MakeString"))
            {
                return string.Empty;
            }

            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = MakeByte();
            }

            return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
        }

        public string MakeFixedString()
        {
            int length = GetStringCount();

            if (!_command.ValidateRemainingArgs(length, "MakeFixedString"))
            {
                return string.Empty;
            }

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

        #region ===== MÉTHODES DE TAILLE ===== 
        public int SizeOfPoint()
        {
            return 2 * SizeOfVdc();
        }

        public int SizeOfVdc()
        {
            if (CgmContext.VdcType == VDCTypeEnum.INTERGER)
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

        #region ===== MÉTHODES BIT ===== 
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

        #region ===== MÉTHODES NON IMPLÉMENTÉES (placeholder) ===== 
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
