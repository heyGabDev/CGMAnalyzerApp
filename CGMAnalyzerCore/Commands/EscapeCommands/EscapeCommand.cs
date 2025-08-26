using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.EscapeCommands
{
    public class EscapeCommand : BaseCgmCommand
    {
        private readonly int _identifier;
        private readonly string _dataRecord;


        public EscapeCommand(int ec, int eid, CgmCommand command, BinaryReader reader, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            _identifier = argReader.MakeInt();
            _dataRecord = argReader.MakeString();

            if (!command.AllArgumentsRead)
            {
                Console.WriteLine($"[Warning] EscapeCommand: all arguments were not read.");
            }

            // Log as unimplemented for now
            Console.WriteLine($"[Unimplemented] EscapeCommand encountered. Identifier = {_identifier}");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"EscapeCommand: identifier = {_identifier}, dataRecord = {_dataRecord}";
        }

    }
}
