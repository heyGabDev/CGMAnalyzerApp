using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class CharacterSetListCommand : CgmCommand
    {
        public enum CharacterSetType
        {
            Char94GSet = 0,
            Char96GSet = 1,
            Char94MByteGSet = 2,
            Char96MByteGSet = 3,
            CompleteCode = 4
        }

        public Dictionary<CharacterSetType, string> CharacterSets { get; private set; }

        public CharacterSetListCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            CharacterSets = new Dictionary<CharacterSetType, string>();

            while (CurrentArg < Args.Length)
            {
                int typ = argReader.MakeEnum();
                CharacterSetType type = typ switch
                {
                    0 => CharacterSetType.Char94GSet,
                    1 => CharacterSetType.Char96GSet,
                    2 => CharacterSetType.Char94MByteGSet,
                    3 => CharacterSetType.Char96MByteGSet,
                    4 => CharacterSetType.CompleteCode,
                    _ => CharacterSetType.CompleteCode
                };

                string characterSetDesignation = argReader.MakeFixedString();
                CharacterSets[type] = characterSetDesignation;
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in CharacterSetList");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("CharacterSetList ");
            foreach (var kvp in CharacterSets)
            {
                sb.Append($"[{kvp.Key},{kvp.Value}]");
            }
            return sb.ToString();
        }
    }

}
