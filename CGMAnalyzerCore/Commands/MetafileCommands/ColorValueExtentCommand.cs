using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGMAnalyzerCore.Commands.MetafileCommands.ColorModelCommand;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class ColorValueExtentCommand : BaseCgmCommand
    {
       // Renommage pour supporter RGB et CMYK
        public int[] MinimumColorValue { get; private set; }
        public int[] MaximumColorValue { get; private set; }

        // Pour les modèles de couleur basés sur des échelles (CIE)
        public double FirstComponentScale { get; private set; }
        public double SecondComponentScale { get; private set; }
        public double ThirdComponentScale { get; private set; }

        public ColorValueExtentCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorValueExtentCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
                var colorModel = CgmContext.ColorModel;

                if (colorModel == ColorModelEnum.RGB || colorModel == ColorModelEnum.CMYK)
                {
                    int precision = CgmContext.ColorPrecision;
                    int componentCount = (colorModel == ColorModelEnum.CMYK) ? 4 : 3;

                    // Lecture des valeurs minimales
                    MinimumColorValue = new int[componentCount];
                    for (int i = 0; i < componentCount; i++)
                    {
                        MinimumColorValue[i] = argReader.MakeUInt(precision);
                    }

                    // Lecture des valeurs maximales
                    MaximumColorValue = new int[componentCount];
                    for (int i = 0; i < componentCount; i++)
                    {
                        MaximumColorValue[i] = argReader.MakeUInt(precision);
                    }

                    // Mettre à jour le contexte global
                    CgmContext.MinimumColorValueRGB = MinimumColorValue;
                    CgmContext.MaximumColorValueRGB = MaximumColorValue;

                    if (colorModel == ColorModelEnum.RGB)
                    {
                        Debug.WriteLine($"[ColorValueExtentCommand] MinRGB=({MinimumColorValue[0]},{MinimumColorValue[1]},{MinimumColorValue[2]}) " +
                                      $"MaxRGB=({MaximumColorValue[0]},{MaximumColorValue[1]},{MaximumColorValue[2]})");
                    }
                    else
                    {
                        Debug.WriteLine($"[ColorValueExtentCommand] MinCMYK=({MinimumColorValue[0]},{MinimumColorValue[1]},{MinimumColorValue[2]},{MinimumColorValue[3]}) " +
                                      $"MaxCMYK=({MaximumColorValue[0]},{MaximumColorValue[1]},{MaximumColorValue[2]},{MaximumColorValue[3]})");
                    }
                }
                else if (colorModel == ColorModelEnum.CIELAB ||
                         colorModel == ColorModelEnum.CIELUV ||
                         colorModel == ColorModelEnum.RGB_RELATED)
                {
                    // Pour les modèles CIE, on lit 6 réels (min et max pour 3 composantes)
                    // Ou selon la spec, 3 facteurs d'échelle
                    FirstComponentScale = argReader.MakeReal();
                    SecondComponentScale = argReader.MakeReal();
                    ThirdComponentScale = argReader.MakeReal();

                    Debug.WriteLine($"[ColorValueExtentCommand] Scales: first={FirstComponentScale}, " +
                                  $"second={SecondComponentScale}, third={ThirdComponentScale}");
                }

                ValidateArgumentsRead("ColorValueExtentCommand");
            }
            catch (Exception)
            {
                // Valeurs par défaut en cas d'erreur
                // Valeurs par défaut en cas d'erreur (RGB 0-255)
                MinimumColorValue = new int[] { 0, 0, 0 };
                MaximumColorValue = new int[] { 255, 255, 255 };
                CgmContext.MinimumColorValueRGB = MinimumColorValue;
                CgmContext.MaximumColorValueRGB = MaximumColorValue;

                FirstComponentScale = 100.0;
                SecondComponentScale = 100.0;
                ThirdComponentScale = 100.0;

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
            sb.Append("COLOUR_VALUE_EXTENT");
            var colorModel = CgmContext.ColorModel;

            if (colorModel == ColorModelEnum.RGB && MinimumColorValue != null && MaximumColorValue != null)
            {
                sb.Append($" min=({MinimumColorValue[0]},{MinimumColorValue[1]},{MinimumColorValue[2]})");
                sb.Append($" max=({MaximumColorValue[0]},{MaximumColorValue[1]},{MaximumColorValue[2]})");
            }
            else if (colorModel == ColorModelEnum.CMYK && MinimumColorValue != null && MaximumColorValue != null)
            {
                sb.Append($" minCMYK=({MinimumColorValue[0]},{MinimumColorValue[1]},{MinimumColorValue[2]},{MinimumColorValue[3]})");
                sb.Append($" maxCMYK=({MaximumColorValue[0]},{MaximumColorValue[1]},{MaximumColorValue[2]},{MaximumColorValue[3]})");
            }
            else if (colorModel == ColorModelEnum.CIELAB ||
                     colorModel == ColorModelEnum.CIELUV ||
                     colorModel == ColorModelEnum.RGB_RELATED)
            {
                sb.Append($" scales=({FirstComponentScale},{SecondComponentScale},{ThirdComponentScale})");
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
