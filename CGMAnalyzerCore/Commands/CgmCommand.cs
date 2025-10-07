using CGMAnalyzerCore.Commands.AttributeCommands;
using CGMAnalyzerCore.Commands.ControlCommands;
using CGMAnalyzerCore.Commands.DelimiterCommands;
using CGMAnalyzerCore.Commands.EscapeCommands;
using CGMAnalyzerCore.Commands.ExternalCommands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.GraphicCommands.Control;
using CGMAnalyzerCore.Commands.MetafileCommands;
using CGMAnalyzerCore.Commands.PictureCommands;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Enums.Core;
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
    /// 1. CgmCommand(constructor) → lit les Args[] depuis BinaryReader
    /// 2. ExtractedArgumentReader → parse les Args[] déjà lus
    /// 3. ReadArguments() → INUTILE, à supprimer

    public class CgmCommand : BaseCgmCommand, ICloneable
    {
        public int ElementClassInt { get; set; }
        public int ElementCode { get; set; }
        public int LayerId { get; set; }
        public bool ErrorCommand { get; set; }
        public int[] Args { get; set; }
        public int CurrentArg { get; set; } = 0;
        public int PosInArg { get; set; } = 0;
        public bool AllArgumentsRead => CurrentArg >= Args?.Length;

        /// <summary>
        /// Lecture & instanciation d'ec, eid, l
        /// </summary>
        /// <param name="ec"></param> // Element class = ec
        /// <param name="eid"></param> // Element ID = eid
        /// <param name="l"></param> // l The number of arguments for the command
        /// <param name="reader"></param> // in The input stream used to read the command
        public CgmCommand(int ec, int eid, int l, BinaryReader reader) : base(ec, eid, l)
        {
            ElementClassInt = ec;
            ElementCode = eid;
            LayerId = CgmContext.CurrentLayerId;
            ErrorCommand = false;
            CurrentArg = 0;  // Initialiser CurrentArg à 0
            PosInArg = 0;

            if (reader != null)
            {
                if (l != 31)
                {
                    // ===== FORME COURTE =====
                    // IMPORTANT : Args doit contenir des OCTETS, pas des UInt16
                    Args = new int[l];

                    try
                    {
                        // Vérifier qu'on a assez de données
                        long remainingBytes = reader.BaseStream.Length - reader.BaseStream.Position;
                        int bytesToRead = Math.Min(l, (int)remainingBytes);

                        // Lire les octets disponibles
                        for (int i = 0; i < bytesToRead; i++)
                        {
                            Args[i] = reader.ReadByte();
                        }

                        // Les octets non lus restent à 0
                        if (bytesToRead < l)
                        {
                            Debug.WriteLine($"[CGM] Commande {ec}:{eid} - Lu {bytesToRead}/{l} octets (fin de stream)");
                        }

                        // Gestion du padding - alignement sur frontière de MOT (2 octets)
                        if (l % 2 == 1 && reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            try
                            {
                                reader.ReadByte(); // Un seul octet de padding
                            }
                            catch (EndOfStreamException)
                            {
                                // Fin de fichier pendant le padding - pas grave
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        Debug.WriteLine($"[CGM] Fin de stream atteinte pour commande {ec}:{eid}");
                    }
                }
                else
                {
                    // ===== FORME LONGUE (argCount == 31) =====
                    bool done = false;
                    List<int> argsList = new List<int>();

                    try
                    {
                        do
                        {
                            // Lire la longueur sur 16 bits (2 octets)
                            int l16 = Read16(reader);

                            if (l16 == -1) break;

                            if ((l16 & 0x8000) != 0) // bit 15 set = pas la dernière partition
                            {
                                done = false;
                                l16 = l16 & 0x7FFF; // Clear bit 15
                            }
                            else
                            {
                                done = true;
                            }

                            // Lire les arguments de cette partition
                            for (int i = 0; i < l16; i++)
                            {
                                argsList.Add(reader.ReadByte());
                            }

                            // Alignement sur frontière de mot si nécessaire
                            if (l16 % 2 == 1 && reader.BaseStream.Position < reader.BaseStream.Length)
                            {
                                try
                                {
                                    reader.ReadByte(); // Un seul octet de padding
                                }
                                catch (EndOfStreamException)
                                {
                                    // Fin de fichier pendant le padding
                                    break;
                                }
                            }
                        }
                        while (!done);
                        Args = argsList.ToArray();
                    }
                    catch (EndOfStreamException)
                    {
                        // Utiliser ce qu'on a pu lire
                        Args = argsList.ToArray();
                        Debug.WriteLine($"[CGM] Erreur lecture forme longue pour commande {ec}:{eid}");
                    }
                }
            }
            else
            {
                // ===== PAS DE READER =====
                // Cas où on appelle le constructeur sans reader (ex: pour copie)
                Args = new int[0];
            }
        }

        /// <summary>
        /// Méthode helper pour lire 16 bits (équivalent au read16 Java)
        /// </summary>
        private int Read16(BinaryReader reader)
        {
            try
            {
                byte b1 = reader.ReadByte();
                byte b2 = reader.ReadByte();
                return (b1 << 8) | b2;
            }
            catch (EndOfStreamException)
            {
                return -1;
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
                byte b1 = reader.ReadByte();
                byte b2 = reader.ReadByte();
                k = (b1 << 8) | b2;
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            int ec = (k >> 12) & 0xF;
            int eid = (k >> 5) & 0x7F;
            int l = k & 0x1F;

            return ReadCommand(reader, ec, eid, l);
        }

        #region ReadElementElement function
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
                    return ReadEscapeElements(reader, ec, eid, l);
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
            }
        }
        #endregion

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
                DelimiterElement.BeginMetafile => new BeginMetafileCommand(ec, eid, l, command, argumentReader),
                // 0, 2 ok
                DelimiterElement.EndMetafile => new EndMetafileCommand(ec, eid, l, command),
                // 0, 3 ok
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
                // 0,19 ok
                DelimiterElement.BeginTileArray => new BeginTileArrayCommand(ec, eid, l, command, argumentReader),
                DelimiterElement.EndTileArray => new EndTileArrayCommand(ec, eid, l, command),
                DelimiterElement.BeginApplicationStructure => new BeginApplicationStructureCommand(ec, eid, l, command, argumentReader),
                // 0,22 ok
                DelimiterElement.BeginApplicationStructureBody => new BeginApplicationStructureBodyCommand(ec, eid, l, command),
                DelimiterElement.EndApplicationStructure => new EndApplicationStructureCommand(ec, eid, l, command),

                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
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
                // 1, 1
                MetafileDescriptorElement.MetafileVersion => new MetafileVersionCommand(ec, eid, l, command, argumentReader),
                // 1, 2
                MetafileDescriptorElement.MetafileDescription => new MetafileDescriptionCommand(ec, eid, l, command, argumentReader),
                // 1, 3
                MetafileDescriptorElement.VdcType => new VDCTypeCommand(ec, eid, l, command, argumentReader),
                // 1, 4
                MetafileDescriptorElement.IntegerPrecision => new IntegerPrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 5
                MetafileDescriptorElement.RealPrecision => new RealPrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 6
                MetafileDescriptorElement.IndexPrecision => new IndexPrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 7
                MetafileDescriptorElement.ColorPrecision => new ColorPrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 8
                MetafileDescriptorElement.ColorIndexPrecision => new ColorIndexPrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 9
                MetafileDescriptorElement.MaximumColorIndex => new MaximumColorIndexCommand(ec, eid, l, command, argumentReader),
                // 1, 10
                MetafileDescriptorElement.ColorValueExtent => new ColorValueExtentCommand(ec, eid, l, command, argumentReader),
                // 1, 11
                MetafileDescriptorElement.MetafileElementList => new MetafileElementListCommand(ec, eid, l, command, argumentReader),
                // 1, 12
                MetafileDescriptorElement.MetafileDefaultsReplacement => new MetafileDefaultsReplacementCommand(ec, eid, l, command, argumentReader),
                // 1, 13
                MetafileDescriptorElement.FontList => new FontListCommand(ec, eid, l, command, argumentReader),
                // 1, 14
                MetafileDescriptorElement.CharacterSetList => new CharacterSetListCommand(ec, eid, l, command, argumentReader),
                // 1, 15
                MetafileDescriptorElement.CharacterCodingAnnouncer => new CharacterCodingAnnouncerCommand(ec, eid, l, command, argumentReader),
                // 1, 16
                MetafileDescriptorElement.NamePrecision => new NamePrecisionCommand(ec, eid, l, command, argumentReader),
                // 1, 17
                MetafileDescriptorElement.MaximumVdcExtent => new MaximumVdcExtentCommand(ec, eid, l, argumentReader),
                // 1, 18
                MetafileDescriptorElement.SegmentPriorityExtent => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 1, 19
                MetafileDescriptorElement.ColorModel => new ColorModelCommand(ec, eid, l, argumentReader),

                // 1, 20 - 24
                MetafileDescriptorElement.FontProperties or
                MetafileDescriptorElement.GlyphMapping or
                MetafileDescriptorElement.SymbolLibraryList or
                MetafileDescriptorElement.PictureDirectory => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 2
        private static BaseCgmCommand ReadPictureDescriptorElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (PictureDescriptorElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                // 2, 1
                PictureDescriptorElement.ScalingMode => new ScalingModeCommand(ec, eid, l, command, argumentReader),
                // 2, 2
                PictureDescriptorElement.ColorSelectionMode => new ColourSelectionModeCommand(ec, eid, l, command, argumentReader),
                // 2, 3
                PictureDescriptorElement.LineWidthSpecificationMode => new LineWidthSpecificationModeCommand(ec, eid, l, command, argumentReader),
                // 2, 4
                PictureDescriptorElement.MarkerSizeSpecificationMode => new MarkerSizeSpecificationModeCommand(ec, eid, l, command, argumentReader),
                // 2, 5
                PictureDescriptorElement.EdgeWidthSpecificationMode => new EdgeWidthSpecificationModeCommand(ec, eid, l, command, argumentReader),
                // 2, 6
                PictureDescriptorElement.VdcExtent => new VDCExtentCommand(ec, eid, l, command, argumentReader),
                // 2, 7
                PictureDescriptorElement.BackgroundColor => new BackgroundColorCommand(ec, eid, l, command, argumentReader),
                // 2, 8
                PictureDescriptorElement.DeviceViewport => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 2, 9
                PictureDescriptorElement.DeviceViewportSpecificationMode => new DeviceViewportSpecificationModeCommand(ec, eid, l, command, argumentReader),
                // 2, 10-15
                PictureDescriptorElement.DeviceViewportMapping or
                PictureDescriptorElement.LineRepresentation or
                PictureDescriptorElement.MarkerRepresentation or
                PictureDescriptorElement.TextRepresentation or
                PictureDescriptorElement.FillRepresentation or
                PictureDescriptorElement.EdgeRepresentation => UnsupportedCommand.Unsupported(ec, eid, l, reader),
                // 2, 16
                PictureDescriptorElement.InteriorStyleSpecificationMode => new InteriorStyleSpecificationModeCommand(ec, eid, l, command, argumentReader),
                // 2, 17
                PictureDescriptorElement.LineAndEdgeTypeDefinition => new LineAndEdgeTypeDefinitionCommand(ec, eid, l, command, argumentReader),
                // 2, 18-20
                PictureDescriptorElement.HatchStyleDefinition or
                PictureDescriptorElement.GeometricPatternDefinition or
                PictureDescriptorElement.ApplicationStructureDirectory => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

        // Class 3
        private static BaseCgmCommand ReadControlElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (ControlElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                ControlElement.VdcIntegerPrecision => new VDCIntegerPrecisionCommand(ec, eid, l, command, argumentReader),
                ControlElement.VdcRealPrecision => new VDCRealPrecisionCommand(ec, eid, l, command, argumentReader),
                ControlElement.ClipRectangle => new ClipRectangleCommand(ec, eid, l, command, argumentReader),
                ControlElement.ClipIndicator => new ClipIndicatorCommand(ec, eid, l, command, argumentReader),

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
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                // 1 - Bundle indices (pas d'implémentation spécifique)
                AttributeElement.LineBundleIndex or
                AttributeElement.MarkerBundleIndex or // 5
                AttributeElement.TextBundleIndex or // 9
                AttributeElement.FillBundleIndex or // 21
                AttributeElement.EdgeBundleIndex => UnsupportedCommand.Unsupported(ec, eid, l, reader), // 26

                // 2-4 - Line attributes
                AttributeElement.LineType => new LineTypeCommand(ec, eid, l, command, argumentReader),
                AttributeElement.LineWidth => new LineWidthCommand(ec, eid, l, command, argumentReader),
                AttributeElement.LineColour => new LineColourCommand(ec, eid, l, command, argumentReader),

                // 6-8 - Marker attributes
                AttributeElement.MarkerType => new MarkerTypeCommand(ec, eid, l, command, argumentReader),
                AttributeElement.MarkerSize => new MarkerSizeCommand(ec, eid, l, command, argumentReader),
                AttributeElement.MarkerColour => new MarkerColourCommand(ec, eid, l, command, argumentReader),

                // 10-20 - Text attributes
                AttributeElement.TextFontIndex => new TextFontIndexCommand(ec, eid, l, command, argumentReader),
                AttributeElement.TextPrecision => new TextPrecisionCommand(ec, eid, l, command, argumentReader),
                AttributeElement.CharacterExpansionFactor => new CharacterExpansionFactorCommand(ec, eid, l, command, argumentReader),
                AttributeElement.CharacterSpacing => new CharacterSpacingCommand(ec, eid, l, command, argumentReader),
                AttributeElement.TextColour => new TextColourCommand(ec, eid, l, command, argumentReader),
                AttributeElement.CharacterHeight => new CharacterHeightCommand(ec, eid, l, command, argumentReader),
                AttributeElement.CharacterOrientation => new CharacterOrientationCommand(ec, eid, l, command, argumentReader),
                AttributeElement.TextPath => new TextPathCommand(ec, eid, l, command, argumentReader),
                AttributeElement.TextAlignment => new TextAlignmentCommand(ec, eid, l, command, argumentReader),
                AttributeElement.CharacterSetIndex => new CharacterSetIndexCommand(ec, eid, l, command, argumentReader),
                AttributeElement.AlternateCharacterSetIndex => new AlternateCharacterSetIndexCommand(ec, eid, l, command, argumentReader),

                // 22-25 - Fill attributes
                AttributeElement.InteriorStyle => new InteriorStyleCommand(ec, eid, l, command, argumentReader),
                AttributeElement.FillColour => new FillColourCommand(ec, eid, l, command, argumentReader),
                AttributeElement.HatchIndex => new HatchIndexCommand(ec, eid, l, command, argumentReader),
                AttributeElement.PatternIndex => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 27-30 - Edge attributes
                AttributeElement.EdgeType => new EdgeTypeCommand(ec, eid, l, command, argumentReader),
                AttributeElement.EdgeWidth => new EdgeWidthCommand(ec, eid, l, command, argumentReader),
                AttributeElement.EdgeColour => new EdgeColourCommand(ec, eid, l, command, argumentReader),
                AttributeElement.EdgeVisibility => new EdgeVisibilityCommand(ec, eid, l, command, argumentReader),

                // 31-33 - Pattern attributes (non supportés)
                AttributeElement.FillReferencePoint or
                AttributeElement.PatternTable or
                AttributeElement.PatternSize => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 34 - Colour table
                AttributeElement.ColourTable => new ColourTableCommand(ec, eid, l, command, argumentReader),

                // 35-36 - Misc attributes (non supportés)
                AttributeElement.AspectSourceFlags or
                AttributeElement.PickIdentifier => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 37-38 - Line caps and joins
                AttributeElement.LineCap => new LineCapCommand(ec, eid, l, command, argumentReader),
                AttributeElement.LineJoin => new LineJoinCommand(ec, eid, l, command, argumentReader),

                // 39-41 - Line continuation (non supportés)
                AttributeElement.LineTypeContinuation or
                AttributeElement.LineTypeInitialOffset or
                AttributeElement.TextScoreType => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 42 - Restricted text type
                AttributeElement.RestrictedTextType => new RestrictedTextTypeCommand(ec, eid, l, command, argumentReader),

                // 43 - Interpolated interior (non supporté)
                AttributeElement.InterpolatedInterior => UnsupportedCommand.Unsupported(ec, eid, l, reader),

                // 44-45 - Edge caps and joins
                AttributeElement.EdgeCap => new EdgeCapCommand(ec, eid, l, command, argumentReader),
                AttributeElement.EdgeJoin => new EdgeJoinCommand(ec, eid, l, command, argumentReader),

                // 46-51 - Edge continuation et symbol attributes (non supportés)
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
        private static BaseCgmCommand ReadEscapeElements(BinaryReader reader, int ec, int eid, int l)
        {
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);
            return new EscapeCommand(ec, eid, l, command, argumentReader);
        }

        // Class 7
        private static BaseCgmCommand ReadExternalElements(BinaryReader reader, int ec, int eid, int l)
        {
            var element = (ExternalElement)eid;
            var command = new CgmCommand(ec, eid, l, reader);
            var argumentReader = new ExtractedArgumentReader(command);

            return element switch
            {
                ExternalElement.Message => new MessageCommand(ec, eid, l, command, argumentReader),
                ExternalElement.ApplicationData => new ApplicationDataCommand(ec, eid, l, command, argumentReader),
                _ => UnsupportedCommand.Unsupported(ec, eid, l, reader)
            };
        }

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

        public override void ReadArguments(BinaryReader reader) { }

        public override void Draw(Graphics g, Pen pen)
        {
            if (ErrorCommand)
            {
                Debug.WriteLine($"[CGM] Commande en erreur, pas de dessin: {GetType().Name}");
                return;
            }
        }

        //public virtual string ToStringDetail()
        //{
        //    return $"{GetType().Name} - EC:{ElementClassInt} EID:{ElementCode}";
        //}

        internal int NextArg()
        {
            if (CurrentArg >= Args.Length)
            {
                // Au lieu de lancer une exception, retourner 0 et logger => Valeur par défaut sûre
                Debug.WriteLine($"[CGM] Tentative de lecture au-delà des arguments disponibles dans {GetType().Name} ({CurrentArg}/{Args.Length})");
                return 0;
            }
            return Args[CurrentArg++];
        }

        internal void SkipBits()
        {
            //if (PosInArg % 8 != 0) //this.posInArg
            if (PosInArg != 0)
            {
                // we read some bits from the current arg but aren't done, skip the rest
                PosInArg = 0; //this.posInArg = 0;
                CurrentArg++; // this.currentArg++;
            }
        }

        /// <summary>
        /// Réinitialise la position de lecture
        /// </summary>
        public void ResetPosition()
        {
            CurrentArg = 0;
            PosInArg = 0;
        }

        /// <summary>
        /// Vérifie s'il reste des arguments à lire
        /// </summary>
        public bool HasMoreArgs()
        {
            return CurrentArg < Args.Length;
        }

        /// <summary>
        /// Nombre d'octets restants
        /// </summary>
        public int RemainingArgs()
        {
            return Args.Length - CurrentArg;
        }

        #region Logger
        protected void ValidateArgumentsRead(string commandName)
        {
            if (CurrentArg != Args.Length)
            {
                Debug.WriteLine($"[CGM] Warning: {commandName} read {CurrentArg}/{Args.Length} arguments");
            }
        }

        public override void LogCommandInfo()
        {
            Debug.WriteLine($"[CGM] Command {GetType().Name} - EC:{ElementClassInt} EID:{ElementCode} " +
                     $"ArgsTotal:{Args?.Length ?? 0} CurrentArg:{CurrentArg} " +
                     $"Remaining:{RemainingArgs()} HasMore:{HasMoreArgs()}");
        }
        #endregion
    }
}
