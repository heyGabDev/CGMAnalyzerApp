using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class CharacterCodingAnnouncerCommand : CgmCommand
    {
        public enum CodingType
        {
            Basic7Bit = 0,
            Basic8Bit = 1,
            Extended7Bit = 2,
            Extended8Bit = 3
        }

        public CodingType Type { get; private set; }

        public CharacterCodingAnnouncerCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int typ = argReader.MakeEnum();
            Type = typ switch
            {
                0 => CodingType.Basic7Bit,
                1 => CodingType.Basic8Bit,
                2 => CodingType.Extended7Bit,
                3 => CodingType.Extended8Bit,
                _ => CodingType.Basic7Bit
            };

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in CharacterCodingAnnouncer");
        }

        public override string ToString()
        {
            return $"CharacterCodingAnnouncer type={Type}";
        }
    }
}
