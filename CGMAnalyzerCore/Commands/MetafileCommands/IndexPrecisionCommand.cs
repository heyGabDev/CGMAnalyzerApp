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
    public class IndexPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; private set; }
        private const int DEFAULT_PRECISION = 16;
        public IndexPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[IndexPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Precision = argReader.MakeInt();
                CgmContext.IndexPrecision = Precision;
                Debug.WriteLine($"[IndexPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("IndexPrecisionCommand");
            }
            catch (Exception ex)
            { 
                Debug.WriteLine($"[IndexPrecisionCommand ERROR] {ex.Message}");
                Precision = DEFAULT_PRECISION; 
                CgmContext.IndexPrecision = Precision;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"INDEX_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
