using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class ColorSelectionModeCommand : BaseCgmCommand
    {
        public enum ColorSelectionType
        {
            INDEXED = 0,
            DIRECT = 1
        }

        public ColorSelectionType Type { get; private set; }
        private const ColorSelectionType DEFAULT_TYPE = ColorSelectionType.INDEXED;

        public ColorSelectionModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorSelectionModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
               var argReader = new ExtractedArgumentReader(this);
                int e = argReader.MakeEnum();
                Type = e switch
                {
                    0 => ColorSelectionType.INDEXED,
                    1 => ColorSelectionType.DIRECT,
                    _ => ColorSelectionType.INDEXED
                };

                // Mettre à jour le contexte global si nécessaire
                CgmContext.ColorSelectionMode = Type;
                Debug.WriteLine($"[ColorSelectionModeCommand] Type={Type}");
                ValidateArgumentsRead("ColorSelectionModeCommand"); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ColorSelectionModeCommand ERROR] {ex.Message}");
                Type = DEFAULT_TYPE;
                CgmContext.ColorSelectionMode = Type;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"COLOR_SELECTION_MODE : {Type}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
