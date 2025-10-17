using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class ColorPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; }
        private const int DEFAULT_PRECISION = 8;

        public ColorPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Precision = argReader.MakeInt();
                CgmContext.ColorPrecision = Precision;
                Debug.WriteLine($"[ColorPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("ColorPrecisionCommand");
            }
            catch (Exception)
            {
                Precision = DEFAULT_PRECISION;
                CgmContext.ColorPrecision = Precision;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"COLOR_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }

        //public static void Reset()
        //{
        //    CgmContext.ColorPrecision = 8;
        //}
    }
}
