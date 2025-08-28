using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Render
{
    public class RenderOptions
    {   
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public bool HighQuality { get; set; } = true;
        public Color BackgroundColor { get; set; } = Color.White;
        public List<string>? VisibleLayers { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
        public bool EnableDebugging { get; set; } = false;
    }
}

