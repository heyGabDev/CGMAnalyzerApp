using CGMAnalyzerCore.Converter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter
{
    public class SimpleNameExtractor : IBeginMetafileNameExtractor
    {
        public string GetFileName(byte[] data, int index)
        {
            return $"part_{index}.cgm";
        }
    }
}
