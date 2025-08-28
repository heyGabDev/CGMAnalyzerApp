using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Modeles
{
    public class CgmMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CommandCount { get; set; }
        public string Version { get; set; } = "Unknown";
        public List<string> UsedCommands { get; set; } = new();
        public bool IsCompressed { get; set; }

    }
}
