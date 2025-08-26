using CGMAnalyzerCore.Display;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.DelimiterCommands
{
    public class BeginTileArrayCommand : BaseCgmCommand
    {
        public Point2D Position { get; }
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

        public BeginTileArrayCommand(int ec, int eid, int l, ExtractedArgumentReader reader)
            : base(ec, eid, l)
        {
            Position = reader.MakePoint(ec, eid);
            CellPathDirection = reader.MakeEnum();
            LineProgressionDirection = reader.MakeEnum();
            NTilesInPathDirection = reader.MakeInt();
            NTilesInLineDirection = reader.MakeInt();
            NCellsPerTileInPathDirection = reader.MakeInt();
            NCellsPerTileInLineDirection = reader.MakeInt();
            CellSizeInPathDirection = reader.MakeReal();
            CellSizeInLineDirection = reader.MakeReal();
            ImageOffsetInPathDirection = reader.MakeInt();
            ImageOffsetInLineDirection = reader.MakeInt();
            NCellsInPathDirection = reader.MakeInt();
            NCellsInLineDirection = reader.MakeInt();
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
                startPosition,
                NTilesInPathDirection,
                NCellsPerTileInPathDirection,
                NCellsPerTileInLineDirection,
                tileSizeInPathDirection,
                tileSizeInLineDirection
            );

            display.SetTileArrayInfo(tileInfo);
        }

    
        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
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

    }
}
