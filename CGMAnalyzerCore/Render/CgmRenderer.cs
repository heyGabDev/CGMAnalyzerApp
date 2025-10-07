using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.DelimiterCommands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.MetafileCommands;
using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Geometry;
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
        public object InteriorStyle { get; private set; }

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

        /// <summary>
        /// Boucle et appel commandes CGM (stockées dans _commands)
        /// </summary>
        /// <param name="graphics"></param>
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
                case CircleCommand circleCommand:
                    return RenderCircle(graphics, command);

                case CircularArc3PointCloseCommand circularArc3PointCloseCommand:
                    return RenderCircularArc3PointCloseCommand(graphics, command);

                case CircularArc3PointCommand circularArc3PointCommand:
                    return RenderCircularArc3PointCommand(graphics, command);

                //case CircularArcCentreCloseCommand circularArcCentreCloseCommand:
                //    return RenderCircularArcCentreCloseCommand(graphics, command);

                //case CircularArcCentreCommand circularArcCentreReversedCommand:
                //    return RenderCircularArcCentreReversedCommand(graphics, command);

                //case CircularArcCentreReversedCommand circularArcCentreReversedCommand:
                //    return RenderCircularArcCentreReversedCommand(graphics, command);

                //case DisjointPolylineCommand disjointPolylineCommand:
                //    return RenderDisjointPolylineCommand(graphics, command);

                //case EllipseCommand ellipseCommand:
                //    return RenderEllipseCommand(graphics, command);

                case PolyBezierCommand polyBezierCommand:
                    return RenderPolyBezier(graphics, polyBezierCommand);

                case PolygonCommand polygonCommand:
                    return RenderPolygon(graphics, polygonCommand);

                case LineCommand lineCommand:
                    return RenderLine(graphics, lineCommand);

                case PolylineCommand polylineCommand:
                    return RenderPolyline(graphics, polylineCommand);

                case DisjointPolylineCommand disjointPolylineCommand:
                    return RenderDisjointPolyline(graphics, disjointPolylineCommand);

                case EllipticalArcCommand ellipticalArcCommand:
                    return RenderEllipticalArc(graphics, ellipticalArcCommand);

                // Commandes de métadonnées (affectent le contexte)
                case ColorModelCommand colourModelCommand:
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

        #region Méthodes de rendu spécifiques RenderXxx(...)
        private bool RenderCircle(Graphics graphics, BaseCgmCommand command)
        {
            using var pen = _context.CreatePen();

            if (command is CircleCommand circle)
            {
                var center = TransformPoint2D(circle.Center);
                var radius = circle.Radius;

                var scaledRadiusX = (float)(radius * Options.Width / _context.VdcExtent.Width);
                var scaledRadiusY = (float)(radius * Options.Height / _context.VdcExtent.Height);

                var rect = new RectangleF(
                    center.X - scaledRadiusX,
                    center.Y - scaledRadiusY,
                    scaledRadiusX * 2,
                    scaledRadiusY * 2
                );
            }
            command.Draw(graphics, pen);
            return true;
        }

        private bool RenderCircularArc3PointCloseCommand(Graphics graphics, BaseCgmCommand command)
        {
            using var pen = _context.CreatePen();
            command.Draw(graphics, pen);
            return true;
        }

        private bool RenderCircularArc3PointCommand(Graphics graphics, BaseCgmCommand command)
        {
            using var pen = _context.CreatePen();
            command.Draw(graphics, pen);
            return true;
        }

        private bool RenderPolyBezier(Graphics graphics, PolyBezierCommand command)
        {
            try
            {
                // Obtenir les points de contrôle
                var controlPoints = command.GetControlPoints();

                if (controlPoints == null || controlPoints.Count < 4)
                {
                    System.Diagnostics.Debug.WriteLine($"[RenderPolyBezier] Pas assez de points: {controlPoints?.Count ?? 0}");
                    return false;
                }

                using var pen = _context.CreatePen();
                using var path = new GraphicsPath();

                // Transformer les Point2D en PointF
                var transformedPoints = controlPoints
                    .Select(p2d => TransformPoint2D(p2d))
                    .ToList();

                // Analyser le format des points pour déterminer la continuité
                // Si (count - 4) % 3 == 0 : courbes continues
                // Si count % 4 == 0 : courbes discontinues
                bool isContinuous = (controlPoints.Count - 4) % 3 == 0;
                bool isDiscontinuous = controlPoints.Count % 4 == 0;

                if (isContinuous && !isDiscontinuous)
                {
                    // Format continu : 4 points puis 3 points par courbe
                    BuildContinuousBezierPath(path, transformedPoints);
                }
                else if (isDiscontinuous)
                {
                    // Format discontinu : 4 points par courbe
                    BuildDiscontinuousBezierPath(path, transformedPoints);
                }
                else
                {
                    // Format ambigu ou invalide - essayer le continu par défaut
                    System.Diagnostics.Debug.WriteLine($"[RenderPolyBezier] Format ambigu, utilisation du mode continu");
                    BuildContinuousBezierPath(path, transformedPoints);
                }

                // Remplir si nécessaire
                if (_context.FillEnabled && _context.InteriorStyle != null && !_context.InteriorStyle.Equals("Empty"))
                {
                    using var brush = _context.CreateBrush();
                    graphics.FillPath(brush, path);
                }

                // Dessiner le contour
                if (_context.EdgeVisibility)
                {
                    graphics.DrawPath(pen, path);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RenderPolyBezier] Erreur: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// A VOIR UTILITE
        /// Méthode alternative : dessiner comme la méthode Draw originale
        /// Utile pour comparer avec votre implémentation existante
        /// </summary>
        private bool RenderPolyBezierClassic(Graphics graphics, PolyBezierCommand command)
        {
            try
            {
                var controlPoints = command.GetControlPoints();
                if (controlPoints.Count < 4) return false;

                using var pen = _context.CreatePen();

                // Dessiner des courbes de Bézier par groupes de 4 points
                // (comme dans votre méthode Draw originale)
                for (int i = 0; i <= controlPoints.Count - 4; i += 3)
                {
                    if (i + 3 < controlPoints.Count)
                    {
                        var p0 = TransformPoint2D(controlPoints[i]);
                        var p1 = TransformPoint2D(controlPoints[i + 1]);
                        var p2 = TransformPoint2D(controlPoints[i + 2]);
                        var p3 = TransformPoint2D(controlPoints[i + 3]);

                        graphics.DrawBezier(pen, p0, p1, p2, p3);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RenderPolyBezierClassic] Erreur: {ex.Message}");
                return false;
            }
        }

        private bool RenderPolygon(Graphics graphics, PolygonCommand command)
        {
            using var pen = _context.CreatePen();
            using var brush = _context.CreateBrush();

            var points = command.GetPoints()?.Select(TransformPoint).ToArray();
            if (points?.Length >= 3)
            {
                graphics.FillPolygon(brush, points);
                graphics.DrawPolygon(pen, points);
                return true;
            }

            return false;
        }

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
        /// <summary>
        /// Transformation d'un Point2D en coordonnées écran avec précision maximale
        /// </summary>
        private PointF TransformPoint2D(Point2D point2d)
        {
            // Calculs en double précision pour conserver la précision
            double x = (point2d.X - _context.VdcExtent.Left) * Options.Width / _context.VdcExtent.Width;
            double y = (point2d.Y - _context.VdcExtent.Top) * Options.Height / _context.VdcExtent.Height;

            // Conversion en float uniquement pour GDI+
            return new PointF((float)x, (float)y);
        }

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

        private SizeF TransformSize(double vdcWidth, double vdcHeight)
        {
            return new SizeF(
                (float)(vdcWidth * Options.Width / _context.VdcExtent.Width),
                (float)(vdcHeight * Options.Height / _context.VdcExtent.Height)
            );
        }

        #endregion

        private void UpdateColourModel(ColorModelCommand command)
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


        #region Aide au rendu de Bézier 
        /// <summary>
        /// Construction d'un chemin de Bézier continu
        /// Format : P0,P1,P2,P3 puis P4,P5,P6 puis P7,P8,P9 etc.
        /// </summary>
        private void BuildContinuousBezierPath(GraphicsPath path, List<PointF> points)
        {
            if (points.Count < 4) return;

            path.StartFigure();

            // Première courbe de Bézier (4 points)
            path.AddBezier(
                points[0],  // P0: point de départ
                points[1],  // P1: premier point de contrôle
                points[2],  // P2: deuxième point de contrôle
                points[3]   // P3: point d'arrivée
            );

            // Courbes suivantes (3 points supplémentaires chacune)
            // Le point de départ est le point d'arrivée de la courbe précédente
            int index = 4;
            while (index + 2 < points.Count)
            {
                path.AddBezier(
                    points[index - 1], // Dernier point de la courbe précédente
                    points[index],     // Premier point de contrôle
                    points[index + 1], // Deuxième point de contrôle
                    points[index + 2]  // Point d'arrivée
                );
                index += 3;
            }
        }

        /// <summary>
        /// Construction d'un chemin de Bézier discontinu
        /// Format : P0,P1,P2,P3 puis P4,P5,P6,P7 puis P8,P9,P10,P11 etc.
        /// </summary>
        private void BuildDiscontinuousBezierPath(GraphicsPath path, List<PointF> points)
        {
            if (points.Count < 4) return;

            path.StartFigure();

            // Chaque courbe nécessite 4 points complets
            for (int i = 0; i <= points.Count - 4; i += 4)
            {
                path.AddBezier(
                    points[i],      // P0: point de départ
                    points[i + 1],  // P1: premier point de contrôle
                    points[i + 2],  // P2: deuxième point de contrôle
                    points[i + 3]   // P3: point d'arrivée
                );
            }
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
