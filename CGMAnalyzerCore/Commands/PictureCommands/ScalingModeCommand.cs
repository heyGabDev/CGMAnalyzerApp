using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public class ScalingModeCommand : CgmCommand
    {
        public enum ScalingModeType
        {
            ABSTRACT = 0,
            METRIC = 1
        }

        public ScalingModeType Mode { get; private set; }
        public double MetricScalingFactor { get; private set; }

        public ScalingModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
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

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ScalingMode");
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
    }
}
