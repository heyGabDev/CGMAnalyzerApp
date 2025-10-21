using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// COLOUR_TABLE (case 34) - Définit ou modifie la table de couleurs
    /// </summary>
    public class ColorTableCommand : BaseCgmCommand
    {
        public int StartIndex { get; private set; } = 0;
        private const int DEFAULT_START_INDEX = 0;
        public List<Color> Colors { get; private set; } = new List<Color>(); 

        public ColorTableCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ColorTableCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Lire l'index de départ
                StartIndex = argReader.MakeColorIndex();
                Debug.WriteLine($"[ColorTableCommand] StartIndex: {StartIndex}");

                // Calculer combien de couleurs peuvent être lues
                int precision = CgmContext.ColorPrecision;
                int bytesPerColor = (precision / 8) * 3; // RGB = 3 composants
                int remainingBytes = this.RemainingArgs();
                int colorCount = remainingBytes / bytesPerColor;

                Debug.WriteLine($"[ColorTableCommand] Precision: {precision} bits, BytesPerColor: {bytesPerColor}, ColorCount: {colorCount}");

                // Lire toutes les couleurs
                for (int i = 0; i < colorCount; i++)
                {
                    // Lire les 3 composants RGB
                    int r = ScaleColorComponent(argReader.MakeUInt(precision));
                    int g = ScaleColorComponent(argReader.MakeUInt(precision));
                    int b = ScaleColorComponent(argReader.MakeUInt(precision));

                    Color color = Color.FromArgb(r, g, b);
                    Colors.Add(color);

                    // Ajouter la couleur à la table globale
                    int colorIndex = StartIndex + i;
                    CgmContext.SetColorInTable(colorIndex, color);

                    Debug.WriteLine($"[ColorTableCommand] Color[{colorIndex}]: R={r}, G={g}, B={b}");
                }

                Debug.WriteLine($"[ColorTableCommand] Total colors loaded: {Colors.Count}");
                ValidateArgumentsRead("ColorTableCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ColorTableCommand ERROR] {ex.Message}");
                StartIndex = DEFAULT_START_INDEX;
                Colors.Clear();
                HasReadErrors = true;
            }
        }

        /// <summary>
        /// Met à l'échelle une composante de couleur en fonction des extents définis dans le contexte
        /// </summary>
        private int ScaleColorComponent(int value)
        {
            var min = CgmContext.MinimumColorValueRGB;
            var max = CgmContext.MaximumColorValueRGB;

            // Si les extents sont définis et valides
            if (min != null && max != null && min.Length > 0 && max.Length > 0 && max[0] != min[0])
            {
                // Normaliser la valeur entre min et max vers [0, 255]
                int scaled = (int)(255.0 * (value - min[0]) / (max[0] - min[0]));
                return Math.Min(255, Math.Max(0, scaled));
            }

            // Sinon, clipper directement à [0, 255]
            return Math.Min(255, Math.Max(0, value));
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"COLOR_TABLE : startIndex={StartIndex} colors={Colors.Count}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
