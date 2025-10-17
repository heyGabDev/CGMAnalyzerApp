using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class CharacterSetListCommand : BaseCgmCommand
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

        public CharacterSetListCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterSetListCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                CharacterSets = new Dictionary<CharacterSetType, string>();

                for ( int i = this.CurrentArg; i < Args?.Length; i++)
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

                Debug.WriteLine($"[CharacterSetListCommand] CharacterSets: " +
                                $"{string.Join(", ", CharacterSets.Select(kvp => $"[{kvp.Key},{kvp.Value}]"))}");
                ValidateArgumentsRead("CharacterSetListCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterSetListCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
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

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
