using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.DelimiterCommands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.MetafileCommands;
using CGMAnalyzerCore.Converter.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CGMAnalyzerCore.Render
{
    public class CgmRenderer : IDisposable
    {
        public int Width { get; set; } = 800;  // valeurs par défaut
        public int Height { get; set; } = 600;

        private readonly List<BaseCgmCommand> _commands;
        private RenderContext _context;
        private bool _disposed;

        public RenderOptions Options { get; set; } = new();

        public CgmRenderer(List<BaseCgmCommand> commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _context = new RenderContext();
        }

        public CgmRenderer(List<BaseCgmCommand> commands, RenderOptions options) : this(commands)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        // <summary>
        /// Rendu principal avec gestion avancée
        /// </summary>
        public Bitmap Render()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CgmRenderer));

            var bitmap = new Bitmap(Options.Width, Options.Height, PixelFormat.Format32bppArgb);

            try
            {
                using var graphics = Graphics.FromImage(bitmap);
                ConfigureGraphics(graphics);

                // Initialiser le contexte de rendu
                _context.Reset(graphics, Options);

                // Pré-traitement : analyser les commandes pour optimisations
                PreprocessCommands();

                // Rendu des commandes
                RenderCommands(graphics);

                return bitmap;
            }
            catch
            {
                bitmap?.Dispose();
                throw;
                //        var bmp = new Bitmap(Width, Height);
                //        using var g = Graphics.FromImage(bmp);

                //        g.Clear(Color.White); // fond blanc
                //        var pen = Pens.Black;

                //        foreach (var command in _commands)
                //        {
                //            switch (command)
                //            { 
                //               case DisjointPolylineCommand disjointPolylineCommand:
                //                    disjointPolylineCommand.Draw(g, pen);
                //                    break;

                //                case EllipticalArcCommand ellipticalArcCommand:
                //                    ellipticalArcCommand.Draw(g, pen);
                //                    break;

                //                case LineCommand lineCommand: 
                //                    lineCommand.Draw(g, pen);   
                //                    break;

                //                case PolylineCommand polyline:
                //                    polyline.Draw(g, pen);
                //                    break;

                //                case ColourModelCommand colourModelCommand:
                //                    colourModelCommand.Draw(g, pen); 
                //                break;

                //                case IntegerPrecisionCommand integerPrecisionCommand:
                //                    integerPrecisionCommand.Draw(g, pen);
                //                    break;

                //                case MaximumVdcExtentCommand maximumVdcExtentCommand:
                //                    maximumVdcExtentCommand.Draw(g, pen);
                //                    break;

                //                case MetafileDescriptionCommand metafileDescriptionCommand:
                //                    metafileDescriptionCommand.Draw(g, pen);
                //                    break;

                //                case MetafileVersionCommand metafileVersionCommand:
                //                    metafileVersionCommand.Draw(g, pen);
                //                    break;


                //                default : break;


                //                    // 🔜 Tu ajouteras ici d'autres cas (Polyline, Rectangle, etc.)
                //            }
                //        }

                //        return bmp;
            }
        }

        /// <summary>
        /// Rendu asynchrone pour éviter de bloquer l'UI
        /// </summary>
        public async Task<Bitmap> RenderAsync()
        {
            return await Task.Run(() => Render());
        }

        private void ConfigureGraphics(Graphics graphics)
        {
            graphics.Clear(Options.BackgroundColor);

            if (Options.HighQuality)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
        }

        private void PreprocessCommands()
        {
            // Analyser les commandes pour extraire les informations de dimensions
            foreach (var command in _commands)
            {
                switch (command)
                {
                    case MaximumVdcExtentCommand vdcCommand:
                        // Ajuster les dimensions si nécessaire
                        UpdateRenderDimensions(vdcCommand);
                        break;

                    case MetafileVersionCommand versionCommand:
                        _context.MetafileVersion = ExtractVersion(versionCommand);
                        break;
                }
            }
        }

        private void RenderCommands(Graphics graphics)
        {
            var renderStats = new RenderStats();
            var currentLayer = "default";

            foreach (var command in _commands)
            {
                try
                {
                    // Gestion des layers
                    if (ShouldSkipLayer(currentLayer))
                        continue;

                    // Rendu de la commande
                    var rendered = RenderCommand(graphics, command);

                    if (rendered)
                        renderStats.RenderedCommands++;
                    else
                        renderStats.SkippedCommands++;

                    renderStats.TotalCommands++;
                }
                catch (Exception ex)
                {
                    renderStats.ErrorCommands++;
                    System.Diagnostics.Debug.WriteLine($"[Render Error] {command.GetType().Name}: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"[Render Stats] Total: {renderStats.TotalCommands}, " +
                $"Rendered: {renderStats.RenderedCommands}, " +
                $"Skipped: {renderStats.SkippedCommands}, " +
                $"Errors: {renderStats.ErrorCommands}");
        }

        private bool RenderCommand(Graphics graphics, BaseCgmCommand command)
        {
            switch (command)
            {
                // Commandes graphiques principales
                case LineCommand lineCommand:
                    return RenderLine(graphics, lineCommand);

                case PolylineCommand polylineCommand:
                    return RenderPolyline(graphics, polylineCommand);

                case DisjointPolylineCommand disjointPolylineCommand:
                    return RenderDisjointPolyline(graphics, disjointPolylineCommand);

                case EllipticalArcCommand ellipticalArcCommand:
                    return RenderEllipticalArc(graphics, ellipticalArcCommand);

                // Commandes de métadonnées (affectent le contexte)
                case ColourModelCommand colourModelCommand:
                    UpdateColourModel(colourModelCommand);
                    return true;

                case IntegerPrecisionCommand integerPrecisionCommand:
                    UpdateIntegerPrecision(integerPrecisionCommand);
                    return true;

                case MaximumVdcExtentCommand maximumVdcExtentCommand:
                    UpdateVdcExtent(maximumVdcExtentCommand);
                    return true;

                case MetafileDescriptionCommand descriptionCommand:
                    // Pas de rendu visuel mais traiter si nécessaire
                    return true;

                case MetafileVersionCommand versionCommand:
                    // Déjà traité dans le préprocessing
                    return true;

                // Commande non supportée
                default:
                    return false;
            }
        }

        #region Méthodes de rendu spécifiques

        private bool RenderLine(Graphics graphics, LineCommand command)
        {
            using var pen = _context.CreatePen();

            // Extraire les points (adaptation selon ton implémentation)
            var points = command.GetPoints(); // Méthode à implémenter dans LineCommand

            if (points.Count >= 2)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var p1 = TransformPoint(points[i]);
                    var p2 = TransformPoint(points[i + 1]);
                    graphics.DrawLine(pen, p1, p2);
                }
                return true;
            }

            return false;
        }

        private bool RenderPolyline(Graphics graphics, PolylineCommand command)
        {
            using var pen = _context.CreatePen();

            var points = command.GetPoints()?.Select(TransformPoint).ToArray();

            if (points?.Length >= 2)
            {
                graphics.DrawLines(pen, points);
                return true;
            }

            return false;
        }

        private bool RenderDisjointPolyline(Graphics graphics, DisjointPolylineCommand command)
        {
            using var pen = _context.CreatePen();

            var pointSets = command.GetPointSets(); // Méthode à implémenter

            if (pointSets?.Any() == true)
            {
                foreach (var pointSet in pointSets)
                {
                    var transformedPoints = pointSet.Select(TransformPoint).ToArray();
                    if (transformedPoints.Length >= 2)
                    {
                        graphics.DrawLines(pen, transformedPoints);
                    }
                }
                return true;
            }

            return false;
        }

        private bool RenderEllipticalArc(Graphics graphics, EllipticalArcCommand command)
        {
            using var pen = _context.CreatePen();

            // Implémentation simplifiée - à adapter selon tes besoins
            var bounds = command.GetBounds(); // Méthode à implémenter
            var startAngle = command.GetStartAngle(); // Méthode à implémenter  
            var sweepAngle = command.GetSweepAngle(); // Méthode à implémenter

            if (bounds.HasValue)
            {
                var rect = TransformRectangle(bounds.Value);
                graphics.DrawArc(pen, rect, startAngle, sweepAngle);
                return true;
            }

            return false;
        }

        #endregion

        #region Méthodes de transformation et contexte

        private PointF TransformPoint(System.Drawing.Point point)
        {
            // Transformation VDC vers coordonnées écran
            var x = (float)((point.X - _context.VdcExtent.Left) * Options.Width / _context.VdcExtent.Width);
            var y = (float)((point.Y - _context.VdcExtent.Top) * Options.Height / _context.VdcExtent.Height);

            return new PointF(x, y);
        }

        private RectangleF TransformRectangle(Rectangle rect)
        {
            var topLeft = TransformPoint(new System.Drawing.Point(rect.Left, rect.Top));
            var bottomRight = TransformPoint(new System.Drawing.Point(rect.Right, rect.Bottom));

            return new RectangleF(
                topLeft.X,
                topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y
            );
        }

        private void UpdateColourModel(ColourModelCommand command)
        {
            // Mettre à jour le modèle de couleur du contexte
            _context.ColourModel = command.GetColourModel();
        }

        private void UpdateIntegerPrecision(IntegerPrecisionCommand command)
        {
            _context.IntegerPrecision = command.GetPrecision();
        }

        private void UpdateVdcExtent(MaximumVdcExtentCommand command)
        {
            _context.VdcExtent = command.GetExtent();
        }

        private void UpdateRenderDimensions(MaximumVdcExtentCommand vdcCommand)
        {
            var extent = vdcCommand.GetExtent();

            // Ajuster les dimensions de rendu si nécessaire
            if (extent.Width > 0 && extent.Height > 0)
            {
                var aspectRatio = (double)extent.Width / extent.Height;
                var targetAspectRatio = (double)Options.Width / Options.Height;

                if (Math.Abs(aspectRatio - targetAspectRatio) > 0.1)
                {
                    // Ajuster les dimensions pour maintenir le ratio
                    if (aspectRatio > targetAspectRatio)
                    {
                        Options.Height = (int)(Options.Width / aspectRatio);
                    }
                    else
                    {
                        Options.Width = (int)(Options.Height * aspectRatio);
                    }
                }
            }
        }

        private string ExtractVersion(MetafileVersionCommand versionCommand)
        {
            return versionCommand.GetVersion() ?? "1.0";
        }

        private bool ShouldSkipLayer(string layerName)
        {
            return Options.VisibleLayers != null &&
                   !Options.VisibleLayers.Contains(layerName);
        }

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _context?.Dispose();
                _disposed = true;
            }

        }

    }
}
