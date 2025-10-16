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
    public class NamePrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; } = 16;
        private const int DEFAULT_PRECISION = 16;

        public NamePrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[NamePrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                Precision = argReader.MakeInt();
                CgmContext.NamePrecision = Precision;
                Debug.WriteLine($"[NamePrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("NamePrecisionCommand");
            }
            catch (Exception ex)
            {
                Precision = DEFAULT_PRECISION;
                CgmContext.NamePrecision = Precision;
                Debug.WriteLine($"[NamePrecisionCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"NAME_PRECISION {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
