using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class ColorValueExtentCommand : CgmCommand
    {
        public int[] MinimumColorValueRGB { get; private set; }
        public int[] MaximumColorValueRGB { get; private set; }
        public double FirstComponentScale { get; private set; }
        public double SecondComponentScale { get; private set; }
        public double ThirdComponentScale { get; private set; }

        public ColorValueExtentCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            var colorModel = CgmContext.ColourModel;

            if (colorModel == ColourModelEnum.RGB || colorModel == ColourModelEnum.CMYK)
            {
                int precision = CgmContext.ColorPrecision;

                if (colorModel == ColourModelEnum.RGB)
                {
                    MinimumColorValueRGB = new int[] {
                    argReader.MakeUInt(precision),
                    argReader.MakeUInt(precision),
                    argReader.MakeUInt(precision)
                };
                    MaximumColorValueRGB = new int[] {
                    argReader.MakeUInt(precision),
                    argReader.MakeUInt(precision),
                    argReader.MakeUInt(precision)
                };

                    // Mettre à jour le contexte global
                    CgmContext.MinimumColorValueRGB = MinimumColorValueRGB;
                    CgmContext.MaximumColorValueRGB = MaximumColorValueRGB;
                }
                else
                {
                    // Unsupported color model CMYK for now
                }
            }
            else if (colorModel == ColourModelEnum.CIELAB ||
                     colorModel == ColourModelEnum.CIELUV ||
                     colorModel == ColourModelEnum.RGB_RELATED)
            {
                FirstComponentScale = argReader.MakeReal();
                SecondComponentScale = argReader.MakeReal();
                ThirdComponentScale = argReader.MakeReal();
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ColourValueExtent");
        }

        public static void Reset()
        {
            CgmContext.MinimumColorValueRGB = new int[] { 0, 0, 0 };
            CgmContext.MaximumColorValueRGB = new int[] { 255, 255, 255 };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ColourValueExtent");

            if (CgmContext.ColourModel == ColourModelEnum.RGB)
            {
                sb.Append($" min RGB=({MinimumColorValueRGB[0]},{MinimumColorValueRGB[1]},{MinimumColorValueRGB[2]})");
                sb.Append($" max RGB=({MaximumColorValueRGB[0]},{MaximumColorValueRGB[1]},{MaximumColorValueRGB[2]})");
            }
            else if (CgmContext.ColourModel == ColourModelEnum.CIELAB ||
                     CgmContext.ColourModel == ColourModelEnum.CIELUV ||
                     CgmContext.ColourModel == ColourModelEnum.RGB_RELATED)
            {
                sb.Append($" first={FirstComponentScale} second={SecondComponentScale} third={ThirdComponentScale}");
            }

            return sb.ToString();
        }
    }

}
