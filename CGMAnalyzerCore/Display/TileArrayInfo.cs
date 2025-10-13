using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CGMAnalyzerCore.Geometry;

namespace CGMAnalyzerCore.Display
{
    public class TileArrayInfo
    {
        public Point2D StartPosition { get; }
        public int TilesInPathDirection { get; }
        public int CellsPerTileInPathDirection { get; }
        public int CellsPerTileInLineDirection { get; }
        public double TileSizeInPathDirection { get; }
        public double TileSizeInLineDirection { get; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float CellWidth { get; set; }
        public float CellHeight { get; set; }
        public int[] TileData { get; set; }
        public PointF Position { get; set; }

        public TileArrayInfo(
            Point2D startPosition,
            int tilesInPathDirection,
            int cellsPerTileInPathDirection,
            int cellsPerTileInLineDirection,
            double tileSizeInPathDirection,
            double tileSizeInLineDirection,
            int width, int height, 
            float cellWidth, float cellHeight)
        {
            StartPosition = startPosition;
            TilesInPathDirection = tilesInPathDirection;
            CellsPerTileInPathDirection = cellsPerTileInPathDirection;
            CellsPerTileInLineDirection = cellsPerTileInLineDirection;
            TileSizeInPathDirection = tileSizeInPathDirection;
            TileSizeInLineDirection = tileSizeInLineDirection;
            Width = width;
            Height = height;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public override string ToString()
        {
            return $"TileArrayInfo(Start={StartPosition}, " +
                   $"TilesInPath={TilesInPathDirection}, " +
                   $"CellsPerTileInPath={CellsPerTileInPathDirection}, " +
                   $"CellsPerTileInLine={CellsPerTileInLineDirection}, " +
                   $"TileSizePath={TileSizeInPathDirection}, " +
                   $"TileSizeLine={TileSizeInLineDirection})";
        }
    }
}

