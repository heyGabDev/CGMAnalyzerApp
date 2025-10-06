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
    public class BeginTileArrayCommand : CgmCommand
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
        public string BeginTileArray { get; private set; } = "";

        public BeginTileArrayCommand(int ec, int eid, int l, CgmCommand baseCommand,ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
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

            try
            {
                if (baseCommand.Args != null && baseCommand.Args.Length > 0)
                {
                    // Lire les arguments avec validation
                    BeginTileArray = argReader.ReadString();
                }
                else
                {
                    BeginTileArray = "Default";
                    ErrorCommand = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CGM] Erreur lecture BeginTileArray: {ex.Message}");
                BeginTileArray = "Error";
                ErrorCommand = true;
            }

            ValidateArgumentsRead("BeginTileArray");
            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in BeginTileArray");
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
