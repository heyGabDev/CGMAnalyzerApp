using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Converter.Enum;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands
{
    public class CgmCommand : BaseCgmCommand, ICloneable
    {
        /// <summary>Index actuel dans Args</summary>
        protected internal int CurrentArg = 0;
        /// <summary>Tous les arguments bruts (mots 16 bits)</summary>
        protected internal int[] Args;
        /// <summary>Position du bit dans l'argument courant</summary>
        protected internal int PosInArg = 0;

        protected readonly int ElementClassInt;
        protected readonly int ElementCode;

        public int LayerId;
        public bool ErrorCommand;

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ec"></param> // Element class = ec
        /// <param name="eid"></param> // Element ID = eid
        /// <param name="argCount"></param> // l The number of arguments for the command
        /// <param name="reader"></param> // in The input stream used to read the command
        public CgmCommand(int ec, int eid, int argCount, BinaryReader reader) : base(ec, eid, argCount)
        {
            ElementClassInt = ec;
            ElementCode = eid;
            LayerId = CgmContext.CurrentLayerId;
            ErrorCommand = false;

            Args = new int[argCount];
            for (int i = 0; i < argCount; i++)
            {
                Args[i] = reader.ReadUInt16();
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Par défaut : rien à dessiner
        }

        public virtual string ToStringDetail()
        {
            return $"{GetType().Name} - EC:{ElementClassInt} EID:{ElementCode}";
        }

        internal int NextArg()
        {
            if (CurrentArg >= Args.Length)
                throw new EndOfStreamException("Plus d’arguments disponibles.");
            return Args[CurrentArg++];
        }

        internal void SkipBits()
        {
            if (PosInArg % 8 != 0) //this.posInArg
            {
                // we read some bits from the current arg but aren't done, skip the rest
                PosInArg = 0; //this.posInArg = 0;
                CurrentArg++; // this.currentArg++;
            }
        }

        #region "Make Function"
        ////////////////////////////////////////////////////////////////////////////
        /// Make() : Crée un Point à partir de deux entiers (x, y)
        ////////////////////////////////////////////////////////////////////////////
        //protected float MakeFloatCoord()
        //{
        //    return NextArg();
        //}

        //protected int MakeUInt8()
        //{
        //    int value = NextArg();
        //    return value & 0xFF;
        //}

        //protected string MakeString(int length)
        //{
        //    var bytes = new byte[length];
        //    for (int i = 0; i < length; i++)
        //    {
        //        bytes[i] = (byte)(NextArg() & 0xFF);
        //    }
        //    return Encoding.UTF8.GetString(bytes);
        //}

        //protected byte MakeByte()
        //{
        //    return (byte)(NextArg() & 0xFF);
        //}

        //protected char MakeChar()
        //{
        //    return (char)(NextArg() & 0xFF);
        //}

        //protected short MakeSignedInt16()
        //{
        //    return unchecked((short)(NextArg() & 0xFFFF));
        //}

        //protected int MakeSignedInt24()
        //{
        //    int b1 = NextArg() & 0xFF;
        //    int b2 = NextArg() & 0xFF;
        //    int b3 = NextArg() & 0xFF;
        //    int value = (b1 << 16) | (b2 << 8) | b3;
        //    if ((value & 0x800000) != 0)
        //        value |= unchecked((int)0xFF000000); // sign extend
        //    return value;
        //}

        //protected int MakeSignedInt32()
        //{
        //    int high = NextArg();
        //    int low = NextArg();
        //    return (high << 16) | low;
        //}

        //protected int MakeInt()
        //{
        //    return NextArg();
        //}

        //public Point2D.Double MakePoint()
        //{
        //    return new Point2D.Double(MakeVdc(), MakeVdc());
        //}

        //protected double MakeVdc()
        //{
        //    if (VDCType.GetType() == VDCType.Type.REAL)
        //    {
        //        var precision = VDCRealPrecision.GetPrecision();
        //        switch (precision)
        //        {
        //            case VDCRealPrecision.Type.FixedPoint32Bit:
        //                return MakeFixedPoint32();
        //            case VDCRealPrecision.Type.FixedPoint64Bit:
        //                return MakeFixedPoint64();
        //            case VDCRealPrecision.Type.FloatingPoint32Bit:
        //                return MakeFloatingPoint32();
        //            case VDCRealPrecision.Type.FloatingPoint64Bit:
        //                return MakeFloatingPoint64();
        //            default:
        //                Unsupported($"unsupported precision {precision}");
        //                return MakeFixedPoint32();
        //        }
        //    }

        //    // Assume integer if not real
        //    int intPrecision = VDCIntegerPrecision.GetPrecision();
        //    switch (intPrecision)
        //    {
        //        case 16:
        //            return MakeSignedInt16();
        //        case 24:
        //            return MakeSignedInt24();
        //        case 32:
        //            return MakeSignedInt32();
        //        default:
        //            Unsupported($"unsupported precision {intPrecision}");
        //            return MakeSignedInt16();
        //    }
        //}

        public static void Unsupported(int ec, int eid)
        {
            Console.WriteLine($"[!] Command not supported: EC={ec}, EID={eid}");
        }

        #endregion
        
        public static BaseCgmCommand Read(BinaryReader reader)
        {
            int k;

            try
            {
                k = reader.ReadByte();
                k = (k << 8) | reader.ReadByte();
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            int ec = k >> 12;
            int eid = (k >> 5) & 0x7F;
            int l = k & 0x1F;

            return ReadCommand(reader, ec, eid, l);
        }

        private static BaseCgmCommand ReadCommand(BinaryReader reader, int ec, int eid, int l)
        {
            switch ((ElementClassEnum)ec)
            {
                // Class: 0
                //case ElementClassInt.DelimiterElements:
                //    return ReadDelimiterElements(reader, ec, eid, l);
                //// Class: 1
                //case ElementClassInt.MetafileDescriptorElements:
                //    return ReadMetafileDescriptorElements(reader, ec, eid, l);
                //// Class: 2
                //case ElementClassInt.PictureDescriptorElements:
                //    return ReadPictureDescriptorElements(reader, ec, eid, l);
                //// Class: 3
                //case ElementClassInt.ControlElements:
                //    return ReadControlElements(reader, ec, eid, l);
                // Class: 4
                case ElementClassEnum.GraphicalPrimitiveElements:
                    return ReadGraphicalPrimitiveElements(reader, ec, eid, l);
                // Class: 5
                //case ElementClassInt.AttributeElements:
                //    return ReadAttributeElements(reader, ec, eid, l);
                //// Class: 6
                //case ElementClassInt.EscapeElements:
                //    return new EscapeCommand(ec, eid, l, reader);
                //// Class: 7
                //case ElementClassInt.ExternalElements:
                //    return ReadExternalElements(reader, ec, eid, l);
                //// Class: 8
                //case ElementClassInt.SegmentElements:
                //    return new UnsupportedCommand(ec, eid, l, reader);
                //// Class: 9
                //case ElementClassInt.ApplicationStructureElements:
                // return new ApplicationStructureCommand(ec, eid, l, reader);

                //case ElementClass.GraphicalPrimitiveElements:
                //    if (eid == 1)
                //        return new LineCommand(ec, eid, l, reader);
                //    break;

                default:
                    return UnsupportedCommand.Unsupported(ec, eid, l, reader);
            }
        }

        // TODO: Implémenter les méthodes ReadXXXElements(...)

        // Class: 4
        private static BaseCgmCommand ReadGraphicalPrimitiveElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (GraphicalPrimitiveElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new CgmArgumentReader(command);

            return element switch
            {
                GraphicalPrimitiveElement.Polyline => new PolylineCommand(ec, eid, l, reader),
                GraphicalPrimitiveElement.DisjointPolyline => new DisjointPolylineCommand(ec, eid, command, argumentReader),
                //GraphicalPrimitiveElement.PolyMarker => new PolyMarkerCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Text => new TextCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.RestrictedText => new RestrictedTextCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Polygon => new PolygonCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.PolygonSet => new PolygonSetCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.CellArray => new CellArrayCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Rectangle => new RectangleCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Circle => new CircleCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.CircularArc3Point => new CircularArc3PointCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.CircularArc3PointClose => new CircularArc3PointCloseCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.CircularArcCentre => new CircularArcCentreCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.CircularArcCentreClose => new CircularArcCentreCloseCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Ellipse => new EllipseCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.EllipticalArc => new EllipticalArcCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.EllipticalArcClose => new EllipticalArcCloseCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.PolyBezier => new PolyBezierCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.BitonalTile => new BitonalTileCommand(ec, eid, l, reader),
                //GraphicalPrimitiveElement.Tile => new TileCommand(ec, eid, l, reader),
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }


        /**
	 * Returns the element class for this command
	 * @return An integer representing the class
	 */
        public int GetElementClass()
        {
            return ElementClassInt;
        }


        /**
         * Returns the element ID for this command
         * @return An integer representing the identifier
         */
        public int GetElementCode()
        {
            return ElementCode;
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
