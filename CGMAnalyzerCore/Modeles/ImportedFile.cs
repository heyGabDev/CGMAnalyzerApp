using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Models
{
    public class ImportedFile
    {
        public string FileName { get; set; }
        public string BmpPath { get; set; } // ex: /images/image.bmp
        public List<string> Layers { get; set; } = new();
        public override string ToString() => FileName; // Ce qui sera affiché dans la ListBox
    }
}
