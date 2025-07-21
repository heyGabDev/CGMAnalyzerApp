using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Converter.Interface
{
    public interface IBeginMetafileNameExtractor
    {
        string GetFileName(byte[] data, int index);
    }
}
