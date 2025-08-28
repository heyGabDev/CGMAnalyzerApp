using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Modeles
{
    public class CgmProcessResult
    {
        public string BmpPath { get; set; } = string.Empty;
        public List<string> Layers { get; set; } = new();
        public bool Success { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CgmMetadata? Metadata { get; set; }
    }
}
