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
    /// TEXT_ALIGNMENT (case 18) - Définit l'alignement horizontal et vertical du texte
    /// </summary>
    public enum HorizontalAlignment
    {
        NormalHorizontal = 0,
        Left = 1,
        Centre = 2,
        Right = 3,
        ContinuousHorizontal = 4
    }

    public enum VerticalAlignment
    {
        NormalVertical = 0,
        Top = 1,
        Cap = 2,
        Half = 3,
        Base = 4,
        Bottom = 5,
        ContinuousVertical = 6
    }

    public class TextAlignmentCommand : BaseCgmCommand
    {
        public HorizontalAlignment HAlign { get; private set; } = HorizontalAlignment.NormalHorizontal;
        public VerticalAlignment VAlign { get; private set; } = VerticalAlignment.NormalVertical;
        public double ContinuousHorizontalValue { get; private set; } = 0.0;
        public double ContinuousVerticalValue { get; private set; } = 0.0;

        public TextAlignmentCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[TextAlignmentCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                int hAlign = argReader.MakeEnum();
                int vAlign = argReader.MakeEnum();
                ContinuousHorizontalValue = argReader.MakeReal();
                ContinuousVerticalValue = argReader.MakeReal();

                HAlign = hAlign switch
                {
                    0 => HorizontalAlignment.NormalHorizontal,
                    1 => HorizontalAlignment.Left,
                    2 => HorizontalAlignment.Centre,
                    3 => HorizontalAlignment.Right,
                    4 => HorizontalAlignment.ContinuousHorizontal,
                    _ => HorizontalAlignment.NormalHorizontal
                };

                VAlign = vAlign switch
                {
                    0 => VerticalAlignment.NormalVertical,
                    1 => VerticalAlignment.Top,
                    2 => VerticalAlignment.Cap,
                    3 => VerticalAlignment.Half,
                    4 => VerticalAlignment.Base,
                    5 => VerticalAlignment.Bottom,
                    6 => VerticalAlignment.ContinuousVertical,
                    _ => VerticalAlignment.NormalVertical
                };

                Debug.WriteLine($"[TextAlignmentCommand]");
                ValidateArgumentsRead("TextAlignmentCommand");
            }
            catch (Exception ex )
            {
                Debug.WriteLine($"[TextAlignmentCommand Error] {ex.Message}");
                HAlign = HorizontalAlignment.NormalHorizontal;
                VAlign = VerticalAlignment.NormalVertical;
                ContinuousHorizontalValue = 0.0; 
                ContinuousVerticalValue = 0.0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"TEXT_ALIGNMENT : h={HAlign} v={VAlign} ch={ContinuousHorizontalValue} cv={ContinuousVerticalValue}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
