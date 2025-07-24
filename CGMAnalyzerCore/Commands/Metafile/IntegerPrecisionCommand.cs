using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Metafile
{
    public class IntegerPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; }

        public IntegerPrecisionCommand(int ec, int eid, int length, CgmArgumentReader reader)
            : base(ec, eid, length)
        {
            Precision = reader.NextArg();
            CgmContext.IntegerPrecision = Precision;
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
