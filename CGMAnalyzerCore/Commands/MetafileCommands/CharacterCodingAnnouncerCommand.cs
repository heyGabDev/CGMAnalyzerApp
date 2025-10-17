using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class CharacterCodingAnnouncerCommand : BaseCgmCommand
    {
        public enum CharacterCodingType
        {
            Basic7Bit = 0,
            Basic8Bit = 1,
            Extended7Bit = 2,
            Extended8Bit = 3
        }

        public CharacterCodingType Type { get; private set; }

        public CharacterCodingAnnouncerCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CharacterCodingAnnouncerCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int type = argReader.MakeEnum();

                Type = type switch
                {
                    0 => CharacterCodingType.Basic7Bit,
                    1 => CharacterCodingType.Basic8Bit,
                    2 => CharacterCodingType.Extended7Bit,
                    3 => CharacterCodingType.Extended8Bit,
                    _ => throw new NotSupportedException($"Unsupported character coding type: {type}")
                };

                Debug.WriteLine($"[CharacterCodingAnnouncerCommand] Type={Type}");
                ValidateArgumentsRead("CharacterCodingAnnouncerCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CharacterCodingAnnouncerCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"CHARACTERE_CODING_ANNOUNCER : type={Type}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
