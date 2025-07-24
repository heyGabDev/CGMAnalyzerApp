using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter.Enum
{
    public enum DelimiterElement
    {
        NoOp = 0,
        BeginMetafile = 1,
        EndMetafile = 2,
        BeginPicture = 3,
        BeginPictureBody = 4,
        EndPicture = 5,
        BeginSegment = 6,
        EndSegment = 7,
        BeginFigure = 8,
        EndFigure = 9,
        BeginProtectionRegion = 13,
        EndProtectionRegion = 14,
        BeginCompoundLine = 15,
        EndCompoundLine = 16,
        BeginCompoundTextPath = 17,
        EndCompoundTextPath = 18,
        BeginTileArray = 19,
        EndTileArray = 20,
        BeginApplicationStructure = 21,
        BeginApplicationStructureBody = 22,
        EndApplicationStructure = 23
    }

}
