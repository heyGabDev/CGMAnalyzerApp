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
using static CGMAnalyzerCore.Commands.PictureCommands.ColorSelectionModeCommand;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{    
    /// <summary>
    /// FILL_COLOR (case 23) - Définit 
    /// </summary>
    public class FillColorCommand : BaseCgmCommand
    {
        public Color FillColor { get; private set; } = Color.Black;
        private readonly Color DEFAULT_FILL_COLOR = Color.Black;

        public FillColorCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[FillColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                if (CgmContext.ColorSelectionMode == ColorSelectionType.INDEXED)
                {
                    int colorIndex = argReader.MakeColorIndex();
                    Debug.WriteLine($"[FillColorCommand] Color index: {colorIndex}");

                    // Récupérer la couleur depuis la table de couleurs
                    FillColor = CgmContext.GetColorFromIndex(colorIndex);
                }
                else
                {
                    FillColor = argReader.MakeDirectColor();
                    Debug.WriteLine($"[FillColorCommand] Direct color: R={FillColor.R}, G={FillColor.G}, B={FillColor.B}, A={FillColor.A}");
                }
                CgmContext.FillColor = this.FillColor;

                //DEBUG
                Debug.WriteLine($"[FillColorCommand] FillColor: R={FillColor.R}, G={FillColor.G}, B={FillColor.B}");
                Debug.WriteLine($"[FillColorCommand] Context updated");

                ValidateArgumentsRead("FillColorCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FillColorCommand Error] {ex.Message}");
                FillColor=DEFAULT_FILL_COLOR;
                CgmContext.FillColor = this.FillColor;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"FILL_COLOR: R={FillColor.R}, G={FillColor.G}, B={FillColor.B}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
