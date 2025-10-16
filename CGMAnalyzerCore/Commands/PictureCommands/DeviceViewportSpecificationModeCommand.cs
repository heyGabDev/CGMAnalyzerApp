using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.PictureCommands
{
    public partial class DeviceViewportSpecificationModeCommand : BaseCgmCommand
    {
        public DeviceViewportMode Mode { get; private set; } = DeviceViewportMode.FractionOfDrawingSurface;
        public double MetricScaleFactor { get; private set; } = 1.0;

        public DeviceViewportSpecificationModeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[DeviceViewportSpecificationModeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);
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
                if (Mode == DeviceViewportMode.MillimetersWithScaleFactor)
                {
                    MetricScaleFactor = argReader.MakeReal();
                }
                else
                {
                    MetricScaleFactor = 1.0; // Valeur par défaut
                }
                CgmContext.DeviceViewportSpecificationMode = Mode;
                Debug.WriteLine($"[DeviceViewportSpecificationModeCommand] Mode={Mode}, MetricScaleFactor={MetricScaleFactor}");
                ValidateArgumentsRead("DeviceViewportSpecificationModeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DeviceViewportSpecificationModeCommand ERROR] {ex.Message}");
                Mode = DeviceViewportMode.FractionOfDrawingSurface; // Valeur par défaut
                MetricScaleFactor = 1.0; // Valeur par défaut
                CgmContext.DeviceViewportSpecificationMode = Mode;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"DEVICE_VIEWPORT_SPECIFICATION_MODE : [specifier={Mode}, metricScaleFactor={MetricScaleFactor}]";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
