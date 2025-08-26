
using CGMAnalyzerCore.Commands.DelimiterCommands;
using CGMAnalyzerCore.Commands.EscapeCommands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.GraphicCommands.Control;
using CGMAnalyzerCore.Commands.MetafileCommands;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Converter.Enums;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        /// <summary> Indique si tous les arguments ont été lus </summary>
        public bool AllArgumentsRead => CurrentArg >= Args?.Length;


        protected readonly int ElementClassInt;
        protected readonly int ElementCode;

        public int LayerId;
        public bool ErrorCommand;

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
            var element = (ElementEnums)ec;
            var command = new CgmCommand(ec, eid, l, reader);
            var argReader = new ExtractedArgumentReader(command);

            switch (element)
            {
                // Class: 0
                case ElementEnums.DelimiterElements:
                    return ReadDelimiterElements(reader, ec, eid, l);
                // Class: 1
                case ElementEnums.MetafileDescriptorElements:
                    return ReadMetaFileDescriptorElements(reader, ec, eid, l);
                // Class: 2
                case ElementEnums.PictureDescriptorElements:
                    return ReadPictureDescriptorElements(reader, ec, eid, l);
                // Class: 3
                case ElementEnums.ControlElements:
                    return ReadControlElements(reader, ec, eid, l);
                // Class: 4
                case ElementEnums.GraphicalPrimitiveElements:
                    return ReadGraphicalPrimitiveElements(reader, ec, eid, l);
                // Class: 5
                case ElementEnums.AttributeElements:
                    return ReadAttributeElements(reader, ec, eid, l);
                // Class: 6
                case ElementEnums.EscapeElements:
                    return new EscapeCommand(ec, eid, command, reader, argReader);
                // Class: 7
                case ElementEnums.ExternalElements:
                    return ReadExternalElements(reader, ec, eid, l);
                // Class: 8
                case ElementEnums.SegmentElements:
                    return UnsupportedCommand.Unsupported(ec, eid, l, reader);
                // Class: 9
                case ElementEnums.ApplicationStructureElements:
                return new CgmCommand(ec, eid, l, reader);
                default:
                    return UnsupportedCommand.Unsupported(ec, eid, l, reader);
            }
        }

        #region ReadElementClass function

        // Class: 0 
        private static BaseCgmCommand ReadDelimiterElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (DelimiterElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                // 0, 0
                DelimiterElement.NoOp => new NoOpCommand(ec, eid, l, reader),
                // 0, 1
                DelimiterElement.BeginMetafile => new BeginMetafileCommand(ec, eid, l, reader),
                // 0, 2
                DelimiterElement.EndMetafile => new EndMetafileCommand(ec, eid, l, reader),
                // 0, 3
                DelimiterElement.BeginPicture => new BeginPictureCommand(ec, eid, l, reader, argumentReader),
                // 0, 4
                DelimiterElement.BeginPictureBody => new BeginPictureBodyCommand(ec, eid, l, reader),
                // 0, 5
                DelimiterElement.EndPicture => new EndPictureCommand(ec, eid, l, argumentReader),

                // 0, 6 => 0,9 non supportés explicitement
                DelimiterElement.BeginSegment or
                DelimiterElement.EndSegment or
                DelimiterElement.BeginFigure or
                DelimiterElement.EndFigure or

                // 0, 13
                DelimiterElement.BeginProtectionRegion or
                DelimiterElement.EndProtectionRegion or
                DelimiterElement.BeginCompoundLine or
                DelimiterElement.EndCompoundLine or
                DelimiterElement.BeginCompoundTextPath or
                DelimiterElement.EndCompoundTextPath => new CgmCommand(ec, eid, l, reader),

                // 0, 19
                DelimiterElement.BeginTileArray => new BeginTileArrayCommand(ec, eid, l, argumentReader),
                //DelimiterElement.EndTileArray => new EndTileArrayCommand(ec, eid, l, reader),
                //DelimiterElement.BeginApplicationStructure => new BeginApplicationStructureCommand(ec, eid, l, reader),
                //DelimiterElement.BeginApplicationStructureBody => new BeginApplicationStructureBodyCommand(ec, eid, l, reader),
                //DelimiterElement.EndApplicationStructure => new EndApplicationStructureCommand(ec, eid, l, reader),

                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader) //a modifier avec mss ci-dessous
                //throw new NotSupportedException($"Unsupported DelimiterElement: {element} (eid={eid})")
            };
        }

        // Class: 1
        private static BaseCgmCommand ReadMetaFileDescriptorElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (MetafileDescriptorElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                // 1
                MetafileDescriptorElement.MetafileVersion => new MetafileVersionCommand(ec, eid, l, argumentReader),
                // 2
                MetafileDescriptorElement.MetafileDescription => new MetafileDescriptionCommand(ec, eid, l, argumentReader),
                // 3
                MetafileDescriptorElement.VdcType => new VDCTypeCommand(ec, eid, l, reader),
                // 4
                MetafileDescriptorElement.IntegerPrecision => new IntegerPrecisionCommand(ec, eid, l, argumentReader),
                // 5
                MetafileDescriptorElement.RealPrecision => new RealPrecisionCommand(ec, eid, l, argumentReader),
                // 6
                //MetafileDescriptorElement.IndexPrecision => new IndexPrecisionCommand(ec, eid, l, reader),
                // 7
                //MetafileDescriptorElement.ColourPrecision => new ColourPrecisionCommand(ec, eid, l, reader),
                // 8
                //MetafileDescriptorElement.ColourIndexPrecision => new ColourIndexPrecisionCommand(ec, eid, l, reader),
                // 9
                //MetafileDescriptorElement.MaximumColourIndex => new MaximumColourIndexCommand(ec, eid, l, reader),
                // 10
                //MetafileDescriptorElement.ColourValueExtent => new ColourValueExtentCommand(ec, eid, l, reader),
                // 11
                //MetafileDescriptorElement.MetafileElementList => new MetafileElementListCommand(ec, eid, l, reader),
                // 12
                //MetafileDescriptorElement.MetafileDefaultsReplacement => new MetafileDefaultsReplacementCommand(ec, eid, l, reader),
                // 13
                //MetafileDescriptorElement.FontList => new FontListCommand(ec, eid, l, reader),
                // 14
                //MetafileDescriptorElement.CharacterSetList => new CharacterSetListCommand(ec, eid, l, reader),
                // 15
                //MetafileDescriptorElement.CharacterCodingAnnouncer => new CharacterCodingAnnouncerCommand(ec, eid, l, reader),
                // 16
                //MetafileDescriptorElement.NamePrecision => new NamePrecisionCommand(ec, eid, l, reader),
                // 17
                MetafileDescriptorElement.MaximumVdcExtent => new MaximumVdcExtentCommand(ec, eid, l, argumentReader),

                // 18
                //MetafileDescriptorElement.SegmentPriorityExtent or

                // 19
                MetafileDescriptorElement.ColourModel => new ColourModelCommand(ec, eid, l, argumentReader),
                
                // 20
                //MetafileDescriptorElement.ColourCalibration or
                // 21
                //MetafileDescriptorElement.FontProperties or
                // 22
                //MetafileDescriptorElement.GlyphMapping or
                // 23
                //MetafileDescriptorElement.SymbolLibraryList or
                // 24
                //MetafileDescriptorElement.PictureDirectory => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 2
        private static BaseCgmCommand ReadPictureDescriptorElements(BinaryReader reader, int ec, int eid, int l) {
            var element = (MetafileDescriptorElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 3
        private static BaseCgmCommand ReadControlElements(BinaryReader reader, int ec, int eid, int l) {
            var element = (AttributeElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class: 4
        private static BaseCgmCommand ReadGraphicalPrimitiveElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (GraphicalPrimitiveElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                // 1
                GraphicalPrimitiveElement.Polyline => new PolylineCommand(ec, eid, l, reader),
                // 2
                GraphicalPrimitiveElement.DisjointPolyline => new DisjointPolylineCommand(ec, eid, command, argumentReader),
                // 3
                //GraphicalPrimitiveElement.PolyMarker => new PolyMarkerCommand(ec, eid, l, reader),
                // 4
                //GraphicalPrimitiveElement.Text => new TextCommand(ec, eid, l, reader),
                // 5
                //GraphicalPrimitiveElement.RestrictedText => new RestrictedTextCommand(ec, eid, l, reader),
                // 6
                //GraphicalPrimitiveElement.AppendText => new AppendTextCommand(ec, eid, l, reader),  
                // 7
                //GraphicalPrimitiveElement.Polygon => new PolygonCommand(ec, eid, l, reader),
                // 8 
                //GraphicalPrimitiveElement.PolygonSet => new PolygonSetCommand(ec, eid, l, reader),
                // 9
                //GraphicalPrimitiveElement.CellArray => new CellArrayCommand(ec, eid, l, reader),
                // 10
                //GraphicalPrimitiveElement.GeneralizedDrawingPrimitive => new GeneralizedDrawingPrimitiveCommand(ec, eid, l, reader),
                // 11
                //GraphicalPrimitiveElement.Rectangle => new RectangleCommand(ec, eid, l, reader),
                // 12
                //GraphicalPrimitiveElement.Circle => new CircleCommand(ec, eid, l, reader),
                // 13
                //GraphicalPrimitiveElement.CircularArc3Point => new CircularArc3PointCommand(ec, eid, l, reader),
                // 14
                //GraphicalPrimitiveElement.CircularArc3PointClose => new CircularArc3PointCloseCommand(ec, eid, l, reader),
                // 15
                //GraphicalPrimitiveElement.CircularArcCentre => new CircularArcCentreCommand(ec, eid, l, reader),
                // 16
                //GraphicalPrimitiveElement.CircularArcCentreClose => new CircularArcCentreCloseCommand(ec, eid, l, reader),
                // 17
                //GraphicalPrimitiveElement.Ellipse => new EllipseCommand(ec, eid, l, reader),
                // 18
                GraphicalPrimitiveElement.EllipticalArc => new EllipticalArcCommand(ec, eid, command, argumentReader),
                // 19
                //GraphicalPrimitiveElement.EllipticalArcClose => new EllipticalArcCloseCommand(ec, eid, l, reader),

                // 20 
                //GraphicalPrimitiveElement.CircularArcCentreReversed or,
                // 21 
                //GraphicalPrimitiveElement.ConnectingEdge or,
                // 22 
                //GraphicalPrimitiveElement.HyperbolicArc or,
                // 23
                //GraphicalPrimitiveElement.ParabolicArc or,
                // 24
                //GraphicalPrimitiveElement.NonUniformBSpline or,
                // 25
                //GraphicalPrimitiveElement.NonUniformRationalBSpline => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 26
                //GraphicalPrimitiveElement.PolyBezier => new PolyBezierCommand(ec, eid, l, reader),
                // 28
                //GraphicalPrimitiveElement.BitonalTile => new BitonalTileCommand(ec, eid, l, reader),
                // 29
                //GraphicalPrimitiveElement.Tile => new TileCommand(ec, eid, l, reader),
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class: 5
        private static BaseCgmCommand ReadAttributeElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (AttributeElement)eid;
            var command = new CgmCommand (ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch 
            {
                // 1
                AttributeElement.LineBundleIndex => new CgmCommand(ec, eid, l, reader),
                // 2
                // AttributeElement.LineType => new LineTypeCommand(ec, eid, command, argumentReader),
                // 3
                // AttributeElement.LineWidth => new LineWidthCommand(ec, eid, command, argumentReader),
                // 4
                // AttributeElement.LineColour => new LineColourCommand(ec, eid, command, argumentReader),
                // 5
                //AttributeElement.MarkerBundleIndex => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 6
                //AttributeElement.MarkerType => new MarkerTypeCommand(ec, eid, command, argumentReader),
                // 7
                //AttributeElement.MarkerSize => new MarkerSizeCommand(ec, eid, command, argumentReader),
                // 8
                //AttributeElement.MarkerColour => new MarkerColourCommand(ec, eid, command, argumentReader),
                // 9
                AttributeElement.TextBundleIndex => new CgmCommand(ec, eid, l, reader),
                // 10
                //AttributeElement.TextFontIndex => new TextFontIndexCommand(ec, eid, command, argumentReader),
                // 11
                //AttributeElement.TextPrecision => new TextPrecisionCommand(ec, eid, command, argumentReader),
                // 12
                //AttributeElement.CharacterExpansionFactor => new CharacterExpansionFactorCommand(ec, eid, command, argumentReader),
                // 13
                //AttributeElement.CharacterSpacing => new CharacterSpacingCommand(ec, eid, command, argumentReader),
                // 14
                //AttributeElement.TextColour => new TextColourCommand(ec, eid, command, argumentReader),
                // 15
                //AttributeElement.CharacterHeight => new CharacterHeightCommand(ec, eid, command, argumentReader),
                // 16
                //AttributeElement.CharacterOrientation => new CharacterOrientationCommand(ec, eid, command, argumentReader),
                // 17
                //AttributeElement.TextPath => new TextPathCommand(ec, eid, command, argumentReader),
                // 18
                //AttributeElement.TextAlignment => new TextAlignmentCommand(ec, eid, command, argumentReader),
                // 19
                //AttributeElement.CharacterSetIndex => new CharacterSetIndexCommand(ec, eid, command, argumentReader),
                // 20
                //AttributeElement.AlternateCharacterSetIndex => new AlternateCharacterSetIndexCommand(ec, eid, command, argumentReader),
                // 21
                AttributeElement.FillBundleIndex => new CgmCommand(ec, eid, l, reader),
                // 22
                //AttributeElement.InteriorStyle => new InteriorStyleCommand(ec, eid, command, argumentReader),
                // 23
                //AttributeElement.FillColour => new FillColourCommand(ec, eid, command, argumentReader),
                // 24
                //AttributeElement.HatchIndex => new HatchIndexCommand(ec, eid, command, argumentReader),
                // 25
                //AttributeElement.PatternIndex or
                // 26
                AttributeElement.EdgeBundleIndex => new CgmCommand(ec, eid, l, reader),
                // 27
                //AttributeElement.EdgeType => new EdgeTypeCommand(ec, eid, command, argumentReader),
                // 28
                //AttributeElement.EdgeWidth => new EdgeWidthCommand(ec, eid, command, argumentReader),
                // 29
                //AttributeElement.EdgeColour => new EdgeColourCommand(ec, eid, command, argumentReader),
                // 30
                //AttributeElement.EdgeVisibility => new EdgeVisibilityCommand(ec, eid, command, argumentReader),
                
                // 31–33
                AttributeElement.FillReferencePoint or
                AttributeElement.PatternTable or
                AttributeElement.PatternSize => new CgmCommand(ec, eid, l, reader),

                // 34
                //AttributeElement.ColourTable => new ColourTableCommand(ec, eid, command, argumentReader),
                // 35
                //AttributeElement.AspectSourceFlags or
                //36
                AttributeElement.PickIdentifier => new CgmCommand(ec, eid, l, reader),
                // 37
                //AttributeElement.LineCap => new LineCapCommand(ec, eid, command, argumentReader),
                // 38
                //AttributeElement.LineJoin => new LineJoinCommand(ec, eid, command, argumentReader),
                
                // 39–41
                AttributeElement.LineTypeContinuation or
                AttributeElement.LineTypeInitialOffset or
                AttributeElement.TextScoreType => new CgmCommand(ec, eid, l, reader),

                // 42
                //AttributeElement.RestrictedTextType => new RestrictedTextTypeCommand(ec, eid, command, argumentReader),
                // 43
                AttributeElement.InterpolatedInterior => new CgmCommand(ec, eid, l, reader),
                // 44
                //AttributeElement.EdgeCap => new EdgeCapCommand(ec, eid, command, argumentReader),
                // 45
                //AttributeElement.EdgeJoin => new EdgeJoinCommand(ec, eid, command, argumentReader),

                // 46–51
                AttributeElement.EdgeTypeContinuation or
                AttributeElement.EdgeTypeInitialOffset or
                AttributeElement.SymbolLibraryIndex or
                AttributeElement.SymbolColour or
                AttributeElement.SymbolSize or
                AttributeElement.SymbolOrientation => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 6
        private static BaseCgmCommand ReadEscapeElements(BinaryReader reader, int ec, int eid, int l) {
            var element = (AttributeElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 7
        private static BaseCgmCommand ReadExternalElements(BinaryReader reader, int ec, int eid, int l) {
            var element = (AttributeElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 8
        //private static BaseCgmCommand ReadDelimiterElements(BinaryReader reader, int ec, int eid, int l) {
        //    var element = (AttributeElement)eid;
        //    var command = new CgmCommand(ec, eid, l, reader);
        //    var argumentReader = new CgmArgumentReader(command);

        //    return element switch
        //    {
        //        _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
        //    };
        //}

        // Class 9
        private static BaseCgmCommand ApplicationStructureCommand(BinaryReader reader, int ec, int eid, int l) {
            var element = (AttributeElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        #endregion

        public int GetElementClass()
        {
            return ElementClassInt;
        }

        public int GetElementCode()
        {
            return ElementCode;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
        public override void Draw(Graphics g, Pen pen){}
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
    
    }
}
