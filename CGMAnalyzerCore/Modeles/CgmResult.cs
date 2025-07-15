using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Modeles
{
    public class CGMResult
    {
        public string FileName { get; set; }
        public string BmpPath { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Layers { get; set; } = new();
    }
}
