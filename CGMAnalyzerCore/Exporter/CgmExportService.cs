using CGMAnalyzerCore.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Exporter
{
    public class CgmExportService
    {
        public string ExportCgmFile(byte[] buffer, string outputDir, string fileName)
        {
            return CgmExporter.WriteFile(buffer, outputDir, fileName);
        }
    }
}
