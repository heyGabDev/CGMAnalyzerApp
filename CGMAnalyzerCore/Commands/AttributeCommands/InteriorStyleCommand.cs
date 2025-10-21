using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    /// <summary>
    /// INTERIOR_STYLE (case 22) - Définit le style de remplissage des polygones
    /// </summary>
    public enum InteriorStyleType
    {
        Hollow = 0,
        Solid = 1,
        Pattern = 2,
        Hatch = 3,
        Empty = 4,
        GeometricPattern = 5,
        Interpolated = 6
    }

    public class InteriorStyleCommand : BaseCgmCommand
    {
        public InteriorStyleType Style { get; private set; } = InteriorStyleType.Hollow;
        private const InteriorStyleType DEFAULT_INTERIOR_STYLE_TYPE = InteriorStyleType.Hollow;

        public InteriorStyleCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[InteriorStyleCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int styleValue = argReader.MakeEnum();
                Style = styleValue switch
                {
                    0 => InteriorStyleType.Hollow,
                    1 => InteriorStyleType.Solid,
                    2 => InteriorStyleType.Pattern,
                    3 => InteriorStyleType.Hatch,
                    4 => InteriorStyleType.Empty,
                    5 => InteriorStyleType.GeometricPattern,
                    6 => InteriorStyleType.Interpolated,
                    _ => InteriorStyleType.Hollow
                };
                CgmContext.InteriorStyle = this.Style;
                Debug.WriteLine($"[InteriorStyleCommand] Style : {styleValue} ({Style})");

                ValidateArgumentsRead("InteriorStyleCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[InteriorStyleCommand Error] {ex.Message}");
                Style = DEFAULT_INTERIOR_STYLE_TYPE;
                CgmContext.InteriorStyle = this.Style;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"INTERIOR_STYLE_TYPE : {Style}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
