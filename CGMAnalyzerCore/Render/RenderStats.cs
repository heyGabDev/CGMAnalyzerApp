using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Render
{
    public class RenderStats
    {
        public int TotalCommands { get; set; }
        public int RenderedCommands { get; set; }
        public int SkippedCommands { get; set; }
        public int ErrorCommands { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }

        public TimeSpan RenderDuration => EndTime - StartTime;

        public double SuccessRate => TotalCommands > 0 ? (double)RenderedCommands / TotalCommands * 100 : 0;

        public override string ToString()
        {
            return $"Render Stats: {RenderedCommands}/{TotalCommands} rendered ({SuccessRate:F1}%), " +
                   $"{ErrorCommands} errors, Duration: {RenderDuration.TotalMilliseconds:F0}ms";
        }
    }
}

