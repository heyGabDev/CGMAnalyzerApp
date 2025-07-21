using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCIntegerPrecisionCommand : CgmCommand
    {
        public int Precision { get; }

        public VDCIntegerPrecisionCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length, reader)
        {
            // On lit la précision VDC à partir des arguments (ex. 2 octets codant la taille en bits)
            // Ici, la norme CGM indique souvent 2 octets pour la précision
            Precision = reader.ReadByte() * 8;
           
            // 🔒 Tu peux ajouter une vérification ici si nécessaire
            if (Precision != 16 && Precision != 24 && Precision != 32)
            {
                throw new InvalidDataException($"Unsupported VDC Integer Precision: {Precision} bits");
            }

            // Stockage dans le contexte global pour accès par d'autres commandes
            CgmContext.VdcIntegerPrecision = Precision;
        }

        public override string ToString()
        {
            return $"VDC Integer Precision: {Precision} bits";
        }
    }
}
