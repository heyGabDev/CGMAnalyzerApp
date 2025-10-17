using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public partial class ScalingModeCommand : BaseCgmCommand
    {
        public enum ScalingModeType
        {
            ABSTRACT = 0,
            METRIC = 1
        }

        public ScalingModeType Mode { get; private set; }
        public double MetricScalingFactor { get; private set; } = 1.0;
        private const ScalingModeType DEFAULT_MODE = ScalingModeType.ABSTRACT;
        private const double DEFAULT_METRIC_SCALING_FACTOR = 1.0;

        public ScalingModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ScalingModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int mod = l > 0 ? argReader.MakeEnum() : 0;

                if (mod == 0)
                {
                    Mode = ScalingModeType.ABSTRACT;
                }
                else if (mod == 1)
                {
                    Mode = ScalingModeType.METRIC;
                    MetricScalingFactor = argReader.MakeFloatingPoint();
                }

                Debug.WriteLine($"[ScalingModeCommand] Mode={Mode}, MetricScalingFactor={MetricScalingFactor}");
                ValidateArgumentsRead("ScalingModeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ScalingModeCommand ERROR] {ex.Message}");
                Mode = DEFAULT_MODE;
                MetricScalingFactor = DEFAULT_METRIC_SCALING_FACTOR;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"ScalingMode mode={Mode}");
            if (Mode == ScalingModeType.METRIC)
            {
                sb.Append($" metricScalingFactor={MetricScalingFactor}");
            }
            return sb.ToString();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
