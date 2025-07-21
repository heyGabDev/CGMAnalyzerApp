using System;
using System.Collections.Generic;
using System.Drawing;
using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.GraphicCommands;

namespace CGMAnalyzerCore.Render
{
    public class CgmRenderer
    {
        private readonly List<BaseCgmCommand> _commands;

        public int Width { get; set; } = 800;  // valeurs par défaut
        public int Height { get; set; } = 600;

        public CgmRenderer(List<BaseCgmCommand> commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public Bitmap Render()
        {
            var bmp = new Bitmap(Width, Height);
            using var g = Graphics.FromImage(bmp);

            g.Clear(Color.White); // fond blanc
            var pen = Pens.Black;

            foreach (var command in _commands)
            {
                switch (command)
                {
                    case PolylineCommand polyline:
                        polyline.Draw(g, pen);
                        break;

                        // 🔜 Tu ajouteras ici d'autres cas (Polyline, Rectangle, etc.)
                }
            }

            return bmp;
        }
    }
}
