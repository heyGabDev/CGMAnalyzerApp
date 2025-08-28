using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Messages
{
    public class CgmProcessingMessage
    {
        public string FileName { get; set; } = string.Empty;
        public ProcessingStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Progress { get; set; } // 0-100
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Exception? Error { get; set; }
    }

    public enum ProcessingStatus
    {
        Started,
        Parsing,
        Rendering,
        Completed,
        Failed
    }
}
