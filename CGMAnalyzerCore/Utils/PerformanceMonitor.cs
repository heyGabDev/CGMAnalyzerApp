using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Utils
{
    public class PerformanceMonitor : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _operationName;
        private readonly ILogger? _logger;

        public PerformanceMonitor(string operationName, ILogger? logger = null)
        {
            _operationName = operationName;
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
        }

        public void LogCheckpoint(string checkpointName)
        {
            _logger?.LogDebug("[{Operation}] {Checkpoint}: {ElapsedMs}ms",
                _operationName, checkpointName, _stopwatch.ElapsedMilliseconds);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger?.LogInformation("[{Operation}] Terminé en {ElapsedMs}ms",
                _operationName, _stopwatch.ElapsedMilliseconds);
        }

        public static PerformanceMonitor Start(string operationName, ILogger? logger = null)
        {
            return new PerformanceMonitor(operationName, logger);
        }
    }
}
