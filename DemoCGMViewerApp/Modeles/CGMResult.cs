using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCGMViewerApp.Modeles
{
    public class CGMResult
    {
        public string FileName { get; set; }
        public string BmpPath { get; set; }
        public List<string> Errors { get; set; }

        public CGMResult()
        {
            BmpPath = string.Empty;
            Errors = new List<string>();
        }
        public CGMResult(string bmpPath, List<string> errors)
        {
            BmpPath = bmpPath;
            Errors = errors ?? new List<string>();
        }
    }
}
