using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Parser
{
    using System;
    using System.IO;

    namespace CGMAnalyzerCore.Parser
    {
        /// <summary>
        /// Lit les arguments CGM directement depuis un flux binaire (BinaryReader),
        /// pendant la lecture des commandes à partir du fichier .cgm.
        /// </summary>
        public class StreamArgumentReader
        {
            private readonly BinaryReader _reader;

            public StreamArgumentReader(BinaryReader reader)
            {
                _reader = reader;
            }

            public byte MakeByte()
            {
                return _reader.ReadByte();
            }

            public int MakeUInt8()
            {
                return _reader.ReadByte();
            }

            public int MakeUInt16()
            {
                var high = _reader.ReadByte();
                var low = _reader.ReadByte();
                return (high << 8) + low;
            }

            public int MakeInt()
            {
                return MakeUInt16(); // ou personnaliser selon le contexte
            }

            public string MakeString()
            {
                int length = GetStringCount();
                byte[] bytes = _reader.ReadBytes(length);

                try
                {
                    return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
                }
                catch (ArgumentException)
                {
                    return Encoding.Default.GetString(bytes);
                }
            }

            private int GetStringCount()
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

            // Ajoute d’autres méthodes similaires (MakeVdc, MakePoint, etc.) selon le besoin
        }
    }

    //public class BinaryCgmArgumentReader
    //{
    //        public static byte MakeByte(BinaryReader reader)
    //        {
    //            return reader.ReadByte();
    //        }

    //        public static int MakeUInt8(BinaryReader reader)
    //        {
    //            return reader.ReadByte();
    //        }

    //        public static int MakeUInt16(BinaryReader reader)
    //        {
    //            if (reader.BaseStream.Length - reader.BaseStream.Position >= 2)
    //            {
    //                byte high = reader.ReadByte();
    //                byte low = reader.ReadByte();
    //                return (high << 8) | low;
    //            }

    //            if (reader.BaseStream.Length - reader.BaseStream.Position >= 1)
    //            {
    //                return reader.ReadByte(); // fallback
    //            }

    //            throw new EndOfStreamException("Unexpected end of stream for UInt16.");
    //        }

    //        public static int GetStringCount(BinaryReader reader)
    //        {
    //            int length = MakeUInt8(reader);
    //            if (length == 255)
    //            {
    //                length = MakeUInt16(reader);
    //                if ((length & (1 << 16)) != 0)
    //                {
    //                    length = (length << 16) | MakeUInt16(reader);
    //                }
    //            }
    //            return length;
    //        }

    //        public static string MakeString(BinaryReader reader)
    //        {
    //            int length = GetStringCount(reader);
    //            byte[] bytes = new byte[length];
    //            for (int i = 0; i < length; i++)
    //            {
    //                bytes[i] = MakeByte(reader);
    //            }

    //            try
    //            {
    //                return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
    //            }
    //            catch
    //            {
    //                return Encoding.Default.GetString(bytes);
    //            }
    //        }
    //    }
}
