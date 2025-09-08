using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    // <summary>
    /// Class=1, Element=8
    /// Colour Index Precision Command
    /// </summary>
    public class ColorIndexPrecisionCommand : CgmCommand
    {
        public int Precision { get; private set; }

        public ColorIndexPrecisionCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Precision = argReader.MakeInt();
            CgmContext.ColorIndexPrecision = Precision;

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in ColourIndexPrecision");
        }

        public static void Reset()
        {
            CgmContext.ColorIndexPrecision = 8;
        }

        public override string ToString()
        {
            return $"ColourIndexPrecision {Precision}";
        }
    }

    // TO DELETE : OLD VERSION KEPT FOR REFERENCE
    //public class ColourIndexPrecision : CgmCommand
    //{
    //    public int Precision { get; private set; }

    //    public ColourIndexPrecision(CgmCommand baseCommand, int ec, int eid)
    //        : base(ec, eid, baseCommand.Args.Length, null)
    //    {
    //        Args = baseCommand.Args;
    //        var argReader = new ExtractedArgumentReader(this);

    //        // Lire la précision des index de couleur
    //        Precision = argReader.MakeInt();

    //        // Mettre à jour le contexte global
    //        CgmContext.ColorPrecision = Precision;

    //        // Vérifier que tous les arguments ont été lus
    //        System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
    //            "Not all arguments were read in ColourIndexPrecision");
    //    }

    //    public static void Reset()
    //    {
    //        CgmContext.ColorPrecision = 8; // Valeur par défaut
    //    }

    //    public override string ToString()
    //    {
    //        return $"ColourIndexPrecision {Precision}";
    //    }
    //}
}
