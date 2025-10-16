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
    public class IntegerPrecisionCommand : BaseCgmCommand
    {
        public int Precision { get; }

        public IntegerPrecisionCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[IntegerPrecisionCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                Precision = argReader.MakeInt();

                // Mettre à jour le contexte CGM
                CgmContext.VdcIntegerPrecision = Precision;

                Debug.WriteLine($"[IntegerPrecisionCommand] Precision={Precision}");
                ValidateArgumentsRead("IntegerPrecisionCommand");
            }
            catch (Exception)
            {
                Precision = 16; // Valeur par défaut en cas d'erreur
                CgmContext.SetIntegerPrecision(Precision);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"INTERGER_PRECISION : {Precision}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
