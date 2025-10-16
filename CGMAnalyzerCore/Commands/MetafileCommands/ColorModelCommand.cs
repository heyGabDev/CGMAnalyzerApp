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
    public class ColorModelCommand : BaseCgmCommand
    {
        public enum ColorModelEnum
        {
            RGB = 1,
            CIELAB = 2,
            CIELUV = 3,
            CMYK = 4,
            RGB_RELATED = 5
        }

        public int ColorModel { get; private set; } = (int)ColorModelEnum.RGB;

        public ColorModelCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorModelCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                ColorModel = argReader.MakeInt();
                CgmContext.ColorModel = (ColorModelEnum)ColorModel;
                Debug.WriteLine($"[ColorModelCommand] ColourModel={ColorModel}");
                ValidateArgumentsRead("ColorModelCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ColorModelCommand ERROR] {ex.Message}");
                ColorModel = (int)ColorModelEnum.RGB; // Valeur par défaut
                CgmContext.ColorModel = (ColorModelEnum)ColorModel;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"COLOR_MODEL: {ColorModel} ({(ColorModelEnum)ColorModel})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
