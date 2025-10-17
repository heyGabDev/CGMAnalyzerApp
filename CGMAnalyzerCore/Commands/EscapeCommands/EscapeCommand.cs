using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.EscapeCommands
{
    public class EscapeCommand : BaseCgmCommand
    {
        public int Identifier { get; private set; }
        public string DataRecord { get; private set; } = "";

        public EscapeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EscapeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Identifier = argReader.MakeInt();

                if(this.CurrentArg < Args?.Length)
                {
                    int remaining = Args.Length - CurrentArg;

                    // Lire comme bytes bruts
                    byte[] dataBytes = new byte[remaining];
                    for (int i = 0; i < remaining; i++)
                    {
                        dataBytes[i] = (byte)argReader.MakeByte();
                    }

                    DataRecord = argReader.MakeString();
                }

                CgmContext.LastEscapeIdentifier = Identifier;
                CgmContext.LastEscapeDataRecord = DataRecord;   
                Debug.WriteLine($"[EscapeCommand] Identifier={Identifier} DataRecord={DataRecord}");
                ValidateArgumentsRead("Escape");
            }
            catch (Exception)
            {
                Identifier = -1; // Valeur par défaut en cas d'erreur
                DataRecord = "";
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"ESCAPE : identifier={Identifier} dataRecord={DataRecord}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");

        }
    }
}
