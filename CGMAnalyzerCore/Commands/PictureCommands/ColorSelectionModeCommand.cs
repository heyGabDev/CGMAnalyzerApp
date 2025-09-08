using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public partial class ColourSelectionModeCommand : CgmCommand
    {

        public ColorSelectionType Type { get; private set; }

        public ColourSelectionModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int e = argReader.MakeEnum();
            Type = e switch
            {
                0 => ColorSelectionType.INDEXED,
                1 => ColorSelectionType.DIRECT,
                _ => ColorSelectionType.INDEXED
            };

            // Mettre à jour le contexte global si nécessaire
            CgmContext.ColorSelectionMode = Type;

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ColourSelectionMode");
        }

        public static void Reset()
        {
            CgmContext.ColorSelectionMode = ColorSelectionType.INDEXED;
        }

        public override string ToString()
        {
            return $"ColourSelectionMode {Type}";
        }
    }

}
