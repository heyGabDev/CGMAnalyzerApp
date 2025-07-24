using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Display
{
    public class CgmDisplay
    {
        public float Scale { get; set; } = 1.0f;
        public PointF Offset { get; set; } = PointF.Empty;
        public Color CurrentColor { get; set; } = Color.Black;
        public float LineWidth { get; set; } = 1.0f;

        public Graphics Graphics { get; } // A modifier par la class Graphics2D
        public Pen Pen { get; }

        //private readonly List<string> _messages = new();

       // public IReadOnlyList<string> Messages => _messages;

        public List<BaseCgmCommand> Commands { get; private set; } = new();
        public List<string> Messages { get; private set; } = new();

        private readonly List<ICommandListener> _listeners = new();
        public CgmDisplay() {}

        public CgmDisplay(Graphics g)
        {
            Graphics = g ?? throw new ArgumentNullException(nameof(g));
            Pen = new Pen(CurrentColor, LineWidth);
        }

        public void Read(BinaryReader reader)
        {
            var parser = new CgmParser();

            // Tu peux aussi injecter les listeners si besoin
            foreach (var listener in _listeners)
            {
                parser.AddCommandListener(listener);
            }

            parser.Read(reader);

            // Transfert du résultat
            Commands = parser.Commands;
            Messages = parser.Messages;
        }

        public void AddListener(ICommandListener listener)
        {
            _listeners.Add(listener);
        }

        public void LogMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Messages.Add(message);
        }

        public PointF ScalePoint(float x, float y)
        {
            return new PointF(
                Offset.X + x * Scale,
                Offset.Y + y * Scale
            );
        }

        public void DrawLine(PointF p1, PointF p2)
        {
            Graphics.DrawLine(Pen, ScalePoint(p1.X, p1.Y), ScalePoint(p2.X, p2.Y));
        }

        public void DrawPolyline(IEnumerable<PointF> points)
        {
            var scaledPoints = new List<PointF>();
            foreach (var pt in points)
                scaledPoints.Add(ScalePoint(pt.X, pt.Y));

            if (scaledPoints.Count >= 2)
                Graphics.DrawLines(Pen, scaledPoints.ToArray());
        }

        public void SetColor(Color color)
        {
            CurrentColor = color;
            Pen.Color = color;
        }

        public void SetLineWidth(float width)
        {
            LineWidth = width;
            Pen.Width = width;
        }
    }
}
