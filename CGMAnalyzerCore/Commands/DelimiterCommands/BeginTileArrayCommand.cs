using CGMAnalyzerCore.Display;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class BeginTileArrayCommand : BaseCgmCommand
    {
        public Point2D Position { get; } = new Point2D.Double(0, 0);
        public int CellPathDirection { get; }
        public int LineProgressionDirection { get; }
        public int NTilesInPathDirection { get; }
        public int NTilesInLineDirection { get; }
        public int NCellsPerTileInPathDirection { get; }
        public int NCellsPerTileInLineDirection { get; }
        public double CellSizeInPathDirection { get; }
        public double CellSizeInLineDirection { get; }
        public int ImageOffsetInPathDirection { get; }
        public int ImageOffsetInLineDirection { get; }
        public int NCellsInPathDirection { get; }
        public int NCellsInLineDirection { get; }
        public string BeginTileArray { get; private set; } = "";

        public BeginTileArrayCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[BeginTileArrayCommand] ArgsLength={Args?.Length ?? 0}");

            var argReader = new ExtractedArgumentReader(command);

            try
            {
                if (Args != null && Args.Length > 0)
                {
                    // Lire les arguments avec validation
                    BeginTileArray = argReader.ReadString();
                    Position = argReader.MakePoint();
                    CellPathDirection = argReader.MakeEnum();
                    LineProgressionDirection = argReader.MakeEnum();
                    NTilesInPathDirection = argReader.MakeInt();
                    NTilesInLineDirection = argReader.MakeInt();
                    NCellsPerTileInPathDirection = argReader.MakeInt();
                    NCellsPerTileInLineDirection = argReader.MakeInt();
                    CellSizeInPathDirection = argReader.MakeReal();
                    CellSizeInLineDirection = argReader.MakeReal();
                    ImageOffsetInPathDirection = argReader.MakeInt();
                    ImageOffsetInLineDirection = argReader.MakeInt();
                    NCellsInPathDirection = argReader.MakeInt();
                    NCellsInLineDirection = argReader.MakeInt();

                    Debug.WriteLine($"[BeginTileArrayCommand] BeginTileArray={BeginTileArray}, Position=({Position.X}, {Position.Y}), " +
                        $"CellPathDirection={CellPathDirection}, LineProgressionDirection={LineProgressionDirection}, " +
                        $"NTilesInPathDirection={NTilesInPathDirection}, NTilesInLineDirection={NTilesInLineDirection}, " +
                        $"NCellsPerTileInPathDirection={NCellsPerTileInPathDirection}, NCellsPerTileInLineDirection={NCellsPerTileInLineDirection}, " +
                        $"CellSizeInPathDirection={CellSizeInPathDirection}, CellSizeInLineDirection={CellSizeInLineDirection}, " +
                        $"ImageOffsetInPathDirection={ImageOffsetInPathDirection}, ImageOffsetInLineDirection={ImageOffsetInLineDirection}, " +
                        $"NCellsInPathDirection={NCellsInPathDirection}, NCellsInLineDirection={NCellsInLineDirection}");
                    ValidateArgumentsRead("BeginTileArrayCommand");
                }
                else
                {
                    BeginTileArray = "Default";
                    Debug.WriteLine($"[BeginTileArrayCommand] No arguments provided, using default values.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CGM] Erreur lecture BeginTileArray: {ex.Message}");
                BeginTileArray = "Error";
            }
        }

        public void ApplyToDisplay(CgmDisplay display)
        {
            var startPosition = new Point2D(
                Position.X - ImageOffsetInPathDirection * CellSizeInPathDirection,
                Position.Y + ImageOffsetInLineDirection * CellSizeInLineDirection
            );

            double boundingBoxSizeInPathDirection = NCellsInPathDirection / CellSizeInPathDirection;
            double boundingBoxSizeInLineDirection = NCellsInLineDirection / CellSizeInLineDirection;

            double tileSizeInPathDirection = boundingBoxSizeInPathDirection / NTilesInPathDirection;
            double tileSizeInLineDirection = boundingBoxSizeInLineDirection / NTilesInLineDirection;

            var tileInfo = new TileArrayInfo(
                    startPosition: startPosition,
                    tilesInPathDirection: NTilesInPathDirection,
                    cellsPerTileInPathDirection: NCellsPerTileInPathDirection,
                    cellsPerTileInLineDirection: NCellsPerTileInLineDirection,
                    tileSizeInPathDirection: tileSizeInPathDirection,
                    tileSizeInLineDirection: tileSizeInLineDirection,
                    width: NCellsInPathDirection,
                    height: NCellsInLineDirection,
                    cellWidth: (float)CellSizeInPathDirection,
                    cellHeight: (float)CellSizeInLineDirection
            );

            display.SetTileArrayInfo(tileInfo);
            display.DrawTileArray();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"BeginTileArray [Position={Position}, " +
                   $"CellPathDirection={CellPathDirection}, LineProgressionDirection={LineProgressionDirection}, " +
                   $"NTilesInPathDirection={NTilesInPathDirection}, NTilesInLineDirection={NTilesInLineDirection}, " +
                   $"NCellsPerTileInPathDirection={NCellsPerTileInPathDirection}, NCellsPerTileInLineDirection={NCellsPerTileInLineDirection}, " +
                   $"CellSizeInPathDirection={CellSizeInPathDirection}, CellSizeInLineDirection={CellSizeInLineDirection}, " +
                   $"ImageOffsetInPathDirection={ImageOffsetInPathDirection}, ImageOffsetInLineDirection={ImageOffsetInLineDirection}, " +
                   $"NCellsInPathDirection={NCellsInPathDirection}, NCellsInLineDirection={NCellsInLineDirection}]";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");

        }
    }
}
