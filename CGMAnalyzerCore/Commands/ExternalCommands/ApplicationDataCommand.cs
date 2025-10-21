using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ExternalCommands
{
    /// <summary>
    /// APPLICATION_DATA (case 7, 2) - Données spécifiques à l'application
    /// </summary>
    public class ApplicationDataCommand : BaseCgmCommand
    {
        public int Identifier { get; private set; } = 0;
        public string Data { get; private set; } = "";
        private const int DEFAULT_IDENTIFIER = 0;

        public ApplicationDataCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ApplicationDataCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                Identifier = argReader.MakeInt();
                Data = argReader.MakeString();
                Debug.WriteLine($"[ApplicationDataCommand] : Identifier={Identifier} - Data={Data}");

                ValidateArgumentsRead("ApplicationDataCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ApplicationDataCommand Error] {ex.Message}");
                Identifier = DEFAULT_IDENTIFIER;
                Data = "";
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"APPLICATION_DATA : Identifier={Identifier}, Data='{Data}'";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
