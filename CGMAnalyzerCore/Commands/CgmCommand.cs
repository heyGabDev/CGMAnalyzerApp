
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
using System.Diagnostics;
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

            if (reader != null)
            {
                if (argCount != 31)
                {
                    try
                    {
                        // Vérifier qu'on a assez de bytes disponibles
                        long remainingBytes = reader.BaseStream.Length - reader.BaseStream.Position;
                        int bytesToRead = Math.Min(argCount, (int)remainingBytes);

                        Args = new int[bytesToRead]; // Ajuster la taille

                        for (int i = 0; i < bytesToRead; i++)
                        {
                            Args[i] = reader.ReadByte();
                        }

                        // Skip padding seulement si on peut
                        if (bytesToRead % 2 == 1 && reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            try
                            {
                                reader.ReadByte();
                            }
                            catch (EndOfStreamException)
                            {
                                // Ignore padding error
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        // Si on ne peut rien lire, créer un tableau vide
                        Args = new int[0];
                        Debug.WriteLine($"[CGM] Fin de stream atteinte pour commande {ec}:{eid}");
                    }

                    //    // Lecture normale - OCTETS, pas des mots de 16 bits
                    //    Args = new int[argCount];
                    //    for (int i = 0; i < argCount; i++)
                    //{
                    //    Args[i] = reader.ReadByte(); // 8 bits, pas 16 !
                    //}
                    //// Alignement sur frontière de mot si nombre impair d'octets
                    //if (argCount % 2 == 1)
                    //{
                    //    try
                    //    {
                    //        reader.ReadByte(); // Skip padding
                    //    }
                    //    catch (EndOfStreamException)
                    //    {
                    //        // Fin de fichier, on ignore
                    //    }
                    //}
                }
            else
            {
                // Forme longue (argCount == 31) - commandes partitionnées
                bool done = false;
                List<int> argsList = new List<int>();

                do
                {
                    // Lire la longueur sur 16 bits
                    int l = (reader.ReadByte() << 8) | reader.ReadByte();
                    if (l == -1) break;

                    if ((l & 0x8000) != 0) // bit 15 set = pas la dernière partition
                    {
                        done = false;
                        l = l & 0x7FFF; // Clear bit 15
                    }
                    else
                    {
                        done = true;
                    }

                    // Lire les arguments de cette partition
                    for (int i = 0; i < l; i++)
                    {
                        argsList.Add(reader.ReadByte());
                    }

                    // Alignement si nécessaire
                    if (l % 2 == 1)
                    {
                        reader.ReadByte();
                    }
                }
                while (!done);

                Args = argsList.ToArray();
            }

            }

        }

        // Constructeur de copie - réutilise les arguments déjà lus
        protected CgmCommand(CgmCommand source, int ec, int eid, int l)
            : base(ec, eid, l)
        {
            ElementClassInt = ec;
            ElementCode = eid;
            LayerId = source.LayerId;
            ErrorCommand = source.ErrorCommand;
            Args = source.Args; // Réutilise les arguments
            CurrentArg = 0;
            PosInArg = 0;
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

        public static BaseCgmCommand ReadCommand(BinaryReader reader, int ec, int eid, int l)
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
                    return ReadApplicationStructureElements(reader, ec, eid, l);

                    default: return UnsupportedCommand.Unsupported(ec, eid, l, reader);

                // Clean SGC 
                //case ElementEnums.ApplicationStructureElements:
                //return new CgmCommand(ec, eid, l, reader);
                //default:
                //    return UnsupportedCommand.Unsupported(ec, eid, l, reader);
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
                DelimiterElement.NoOp => command,
                // 0, 1
                DelimiterElement.BeginMetafile => new BeginMetafileCommand(ec, eid, l, command),
                // 0, 2
                DelimiterElement.EndMetafile => new EndMetafileCommand(ec, eid, l, command),
                // 0, 3
                DelimiterElement.BeginPicture => new BeginPictureCommand(ec, eid, l, command),
                // 0, 4
                DelimiterElement.BeginPictureBody => new BeginPictureBodyCommand(ec, eid, l, command),
                // 0, 5
                DelimiterElement.EndPicture => new EndPictureCommand(ec, eid, l, command),

                // 0, 6 => 0,9 et 0,13 => 0,17 non supportés explicitement
                DelimiterElement.BeginSegment or
                DelimiterElement.EndSegment or
                DelimiterElement.BeginFigure or
                DelimiterElement.EndFigure or
                DelimiterElement.BeginProtectionRegion or
                DelimiterElement.EndProtectionRegion or
                DelimiterElement.BeginCompoundLine or
                DelimiterElement.EndCompoundLine or
                DelimiterElement.BeginCompoundTextPath or

                // 0,18
                DelimiterElement.EndCompoundTextPath => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 0,19
                DelimiterElement.BeginTileArray => new BeginTileArrayCommand(ec, eid, l, command, argumentReader),
                DelimiterElement.EndTileArray => new EndTileArrayCommand(ec, eid, l, command),
                DelimiterElement.BeginApplicationStructure => new BeginApplicationStructureCommand(ec, eid, l, command, argumentReader),
                DelimiterElement.BeginApplicationStructureBody => new BeginApplicationStructureBodyCommand(ec, eid, l, command),
                DelimiterElement.EndApplicationStructure => new EndApplicationStructureCommand(ec, eid, l, command),

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
                MetafileDescriptorElement.MetafileVersion => new MetafileVersionCommand(ec, eid, l, command, argumentReader),
                // 2
                MetafileDescriptorElement.MetafileDescription => new MetafileDescriptionCommand(ec, eid, l, command, argumentReader),
                // 3
                MetafileDescriptorElement.VdcType => new VDCTypeCommand(ec, eid, l, command, argumentReader),
                // 4
                MetafileDescriptorElement.IntegerPrecision => new IntegerPrecisionCommand(ec, eid, l, command,argumentReader),
                // 5
                MetafileDescriptorElement.RealPrecision => new RealPrecisionCommand(ec, eid, l, command, argumentReader),
                // 6
                MetafileDescriptorElement.IndexPrecision => new IndexPrecisionCommand(ec, eid, l, command, argumentReader),
                // 7
                MetafileDescriptorElement.ColorPrecision => new ColorPrecisionCommand(ec, eid, l, command, argumentReader),
                // 8
                MetafileDescriptorElement.ColorIndexPrecision => new ColorIndexPrecisionCommand(ec, eid, l, command, argumentReader),
                // 9
                MetafileDescriptorElement.MaximumColorIndex => new MaximumColorIndexCommand(ec, eid, l, command, argumentReader),
                // 10
                MetafileDescriptorElement.ColorValueExtent => new ColorValueExtentCommand(ec, eid, l, command, argumentReader),
                // 11
                MetafileDescriptorElement.MetafileElementList => new MetafileElementListCommand(ec, eid, l, command, argumentReader),
                // 12
                MetafileDescriptorElement.MetafileDefaultsReplacement => new MetafileDefaultsReplacementCommand(ec, eid, l, command, argumentReader),
                // 13
                MetafileDescriptorElement.FontList => new FontListCommand(ec, eid, l, command, argumentReader),
                // 14
                MetafileDescriptorElement.CharacterSetList => new CharacterSetListCommand(ec, eid, l, command, argumentReader),
                // 15
                MetafileDescriptorElement.CharacterCodingAnnouncer => new CharacterCodingAnnouncerCommand(ec, eid, l, command, argumentReader),
                // 16
                MetafileDescriptorElement.NamePrecision => new NamePrecisionCommand(ec, eid, l,command, argumentReader),
                // 17
                MetafileDescriptorElement.MaximumVdcExtent => new MaximumVdcExtentCommand(ec, eid, l, argumentReader),
                // 18
                MetafileDescriptorElement.SegmentPriorityExtent => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 19
                MetafileDescriptorElement.ColorModel => new ColorModelCommand(ec, eid, l, argumentReader),

                // 20 - 24
                MetafileDescriptorElement.FontProperties or
                MetafileDescriptorElement.GlyphMapping or
                MetafileDescriptorElement.SymbolLibraryList or
                MetafileDescriptorElement.PictureDirectory => UnsupportedCommand.Unsupported(ec, eid, l, reader),
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
                GraphicalPrimitiveElement.PolyMarker => new PolyMarkerCommand(ec, eid, command, argumentReader),
                // 4
                GraphicalPrimitiveElement.Text => new TextCommand(ec, eid, command, argumentReader),
                // 5
                GraphicalPrimitiveElement.RestrictedText => new RestrictedTextCommand(ec, eid, command, argumentReader),
                // 6
                GraphicalPrimitiveElement.AppendText => new AppendTextCommand(ec, eid, command, argumentReader),  
                // 7
                GraphicalPrimitiveElement.Polygon => new PolygonCommand(ec, eid, command, argumentReader),
                // 8 
                GraphicalPrimitiveElement.PolygonSet => new PolygonSetCommand(ec, eid, command, argumentReader),
                // 9
                GraphicalPrimitiveElement.CellArray => new CellArrayCommand(ec, eid, command, argumentReader),
                // 10
                GraphicalPrimitiveElement.GeneralizedDrawingPrimitive => new GeneralizedDrawingPrimitiveCommand(ec, eid, command, argumentReader),
                // 11
                GraphicalPrimitiveElement.Rectangle => new RectangleCommand(ec, eid, command, argumentReader),
                // 12
                GraphicalPrimitiveElement.Circle => new CircleCommand(ec, eid, command, argumentReader),
                // 13
                GraphicalPrimitiveElement.CircularArc3Point => new CircularArc3PointCommand(ec, eid, command, argumentReader),
                // 14
                GraphicalPrimitiveElement.CircularArc3PointClose => new CircularArc3PointCloseCommand(ec, eid, command, argumentReader),
                // 15
                GraphicalPrimitiveElement.CircularArcCentre => new CircularArcCentreCommand(ec, eid, command, argumentReader),
                // 16
                GraphicalPrimitiveElement.CircularArcCentreClose => new CircularArcCentreCloseCommand(ec, eid, command, argumentReader),
                // 17
                GraphicalPrimitiveElement.Ellipse => new EllipseCommand(ec, eid, command, argumentReader),
                // 18
                GraphicalPrimitiveElement.EllipticalArc => new EllipticalArcCommand(ec, eid, command, argumentReader),
                // 19
                GraphicalPrimitiveElement.EllipticalArcClose => new EllipticalArcCloseCommand(ec, eid, command, argumentReader),
                // 20 
                GraphicalPrimitiveElement.CircularArcCentreReversed or
                // 21 
                GraphicalPrimitiveElement.ConnectingEdge or
                // 22 
                GraphicalPrimitiveElement.HyperbolicArc or
                // 23
                GraphicalPrimitiveElement.ParabolicArc or
                // 24
                GraphicalPrimitiveElement.NonUniformBSpline or
                // 25
                GraphicalPrimitiveElement.NonUniformRationalBSpline => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 26
                GraphicalPrimitiveElement.PolyBezier => new PolyBezierCommand(ec, eid, command, argumentReader),
                // 28
                GraphicalPrimitiveElement.BitonalTile => new BitonalTileCommand(ec, eid, command, argumentReader),
                // 29
                GraphicalPrimitiveElement.Tile => new TileCommand(ec, eid, command, argumentReader),
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
        private static BaseCgmCommand ReadApplicationStructureElements(BinaryReader reader, int ec, int eid, int l)
        {
            var command = new CgmCommand(ec, eid, l, reader);
            return new ApplicationStructureCommand(command, ec, eid);
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

            //try
            //{
            //    int pointCount = Length / 4; // Chaque point = 2 x Int16 (2*2 octets)

            //    // AJOUTER : Vérifier qu'on ne dépasse pas
            //    long remainingBytes = reader.BaseStream.Length - reader.BaseStream.Position;
            //    int maxPoints = (int)(remainingBytes / 4);
            //    pointCount = Math.Min(pointCount, maxPoints);

            //    for (int i = 0; i < pointCount; i++)
            //    {
            //        int x = reader.ReadInt16();
            //        int y = reader.ReadInt16();
            //        Points.Add(new Point(x, y));
            //    }
            //}
            //catch (EndOfStreamException)
            //{
            //    // Log et continuer
            //    Debug.WriteLine($"EndOfStream dans {GetType().Name}");
            //}
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
