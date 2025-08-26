using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class IntegerPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; }

        public IntegerPrecisionCommand(int ec, int eid, int length, ExtractedArgumentReader argReader)
            : base(ec, eid, length)
        {
            Precision = argReader.NextArg();
            CgmContext.VdcIntegerPrecision = Precision;
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Draw(System.Drawing.Graphics g, System.Drawing.Pen pen)
        {
            // Aucun dessin nécessaire
        }

        public override string ToString()
        {
            return $"IntegerPrecisionCommand: Precision = {Precision}";
        }

    }
}
