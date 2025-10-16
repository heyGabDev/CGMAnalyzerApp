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
    public class BackgroundColorCommand : BaseCgmCommand
    {
        public System.Drawing.Color BackgroundColor { get; private set; } = System.Drawing.Color.White;

        public BackgroundColorCommand(int ec, int eid, int l, CgmCommand baseCommand )
            : base(ec, eid, l)
        {
            Args = baseCommand.Args;
            Debug.WriteLine($"[BackgroundColorCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(baseCommand);
                BackgroundColor = argReader.MakeDirectColor();
                CgmContext.BackgroundColor = BackgroundColor;
                Debug.WriteLine($"[BackgroundColorCommand] BackgroundColor={BackgroundColor}");
                ValidateArgumentsRead("BackgroundColorCommand");
            }
            catch (Exception)
            {
                BackgroundColor = System.Drawing.Color.White; // Valeur par défaut
                CgmContext.BackgroundColor = BackgroundColor;
                Debug.WriteLine($"[BackgroundColorCommand ERROR] Setting default BackgroundColor={BackgroundColor}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"BACKGROUND_COLOR : {BackgroundColor}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
