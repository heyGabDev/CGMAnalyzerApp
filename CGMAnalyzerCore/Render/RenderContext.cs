using CGMAnalyzerCore.Converter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Render
{
    public class RenderContext:IDisposable
    {
        public string MetafileVersion { get; set; } = "1.0";
        public string ColourModel { get; set; } = "RGB";
        public int IntegerPrecision { get; set; } = 16;
        public Rectangle VdcExtent { get; set; } = new Rectangle(0, 0, 32767, 32767);

        private Pen? _currentPen;
        private Brush? _currentBrush;
        private bool _disposed;

        public void Reset(Graphics graphics, RenderOptions options)
        {
            _currentPen?.Dispose();
            _currentBrush?.Dispose();

            _currentPen = new Pen(Color.Black, 1.0f);
            _currentBrush = new SolidBrush(Color.Black);
        }

        public Pen CreatePen()
        {
            return _currentPen?.Clone() as Pen ?? new Pen(Color.Black);
        }

        public Brush CreateBrush()
        {
            return _currentBrush?.Clone() as Brush ?? new SolidBrush(Color.Black);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _currentPen?.Dispose();
                _currentBrush?.Dispose();
                _disposed = true;
            }
        }
    }
}

