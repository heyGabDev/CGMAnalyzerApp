using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Modeles
{
    public class CgmComplexity
    {
        public int TotalCommands { get; set; }
        public int GraphicCommands { get; set; }
        public int AttributeCommands { get; set; }
        public TimeSpan EstimatedRenderTime { get; set; }
        public string ComplexityLevel { get; set; } = "Unknown";
    }
}
