using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public partial class DeviceViewportSpecificationModeCommand : CgmCommand
    {

        public DeviceViewportMode Mode { get; private set; }
        public double MetricScaleFactor { get; private set; }

        public DeviceViewportSpecificationModeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int e = argReader.MakeEnum();
            Mode = e switch
            {
                0 => DeviceViewportMode.FractionOfDrawingSurface,
                1 => DeviceViewportMode.MillimetersWithScaleFactor,
                2 => DeviceViewportMode.PhysicalDeviceCoordinates,
                _ => DeviceViewportMode.FractionOfDrawingSurface
            };

            // Le facteur métrique dépend de si RealPrecision a été traité
            // Simplification : toujours utiliser MakeReal()
            MetricScaleFactor = argReader.MakeReal();

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in DeviceViewportSpecificationMode");
        }

        public static void Reset()
        {
            CgmContext.DeviceViewportSpecificationMode = DeviceViewportMode.FractionOfDrawingSurface;
        }

        public override string ToString()
        {
            return $"DeviceViewportSpecificationMode [specifier={Mode}, metricScaleFactor={MetricScaleFactor}]";
        }
    }
}
