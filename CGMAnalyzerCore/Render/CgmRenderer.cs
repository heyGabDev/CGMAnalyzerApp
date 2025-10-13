using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.DelimiterCommands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Commands.GraphicCommands.Control;
using CGMAnalyzerCore.Commands.MetafileCommands;
using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Enums.Colors;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Rendering;
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

            _context = new RenderContext(Options.Width, Options.Height);
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
                        var version = ExtractVersion(versionCommand);
                        System.Diagnostics.Debug.WriteLine($"CGM Metafile Version: {version}");
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

        private bool RenderCommand(Graphics g, BaseCgmCommand cmd)
        {
            switch (cmd)
            {
                // 1) PRIMITIVES GRAPHIQUES (EC = 4) — dessinent
                // Lignes, polylignes, courbes, polygones
                case LineCommand c: return Draw(g, c);
                case PolylineCommand c: return Draw(g, c);
                case DisjointPolylineCommand c: return Draw(g, c);
                case PolyBezierCommand c: return Draw(g, c);
                case PolygonCommand c: return Draw(g, c);
                case PolygonSetCommand c: return Draw(g, c);
                case RectangleCommand c: return Draw(g, c);

                // Cercles, ellipses, arcs
                case CircleCommand c: return Draw(g, c);
                case EllipseCommand c: return Draw(g, c);
                case EllipticalArcCommand c: return Draw(g, c);
                case EllipticalArcCloseCommand c: return Draw(g, c);
                case CircularArc3PointCommand c: return Draw(g, c);
                case CircularArc3PointCloseCommand c: return Draw(g, c);
                case CircularArcCentreCommand c: return Draw(g, c);
                case CircularArcCentreCloseCommand c: return Draw(g, c);
                case CircularArcCentreReversedCommand c: return Draw(g, c);

                // Points/markers, trames/tiles
                case PolyMarkerCommand c: return Draw(g, c);
                case CellArrayCommand c: return Draw(g, c);
                case BitonalTileCommand c: return Draw(g, c);
                case TileCommand c: return Draw(g, c);

                // Texte
                case TextCommand c: return Draw(g, c);
                case RestrictedTextCommand c: return Draw(g, c);
                case AppendTextCommand c: return Draw(g, c);
            }

            // 2) COMMANDES CONTEXTE/METAFILE — ne dessinent pas, mettent à jour l’état
            switch (cmd)
            {
                // Descripteurs de métafile / VDC / précisions / couleurs
                case MaximumVdcExtentCommand c: UpdateVdcExtent(c); return true;
                case ColorModelCommand c: UpdateColourModel(c); return true;
                case IntegerPrecisionCommand c: UpdateIntegerPrecision(c); return true;
                case RealPrecisionCommand c: CgmContext.RealPrecision = c.GetPrecision(); return true;
                case VDCTypeCommand c: CgmContext.SetVdcType(c.Type); return true;
                case MetafileVersionCommand: return true;
                case MetafileDescriptionCommand: return true;

                // Délimiteurs (begin/end) – pilotent le flux mais ne dessinent pas
                case BeginMetafileCommand: return true;
                case EndMetafileCommand: return true;
                case BeginPictureCommand: return true;
                case BeginPictureBodyCommand: return true;
                case EndPictureCommand: return true;

                // Pas (encore) supporté : on ne dessine pas
                //case UnsupportedCommand: return false;
            }

            // 3) Défaut : on ne sait pas rendre → ignorer (pas d’erreur)
            return false;
        }

        private bool Draw(Graphics g, BaseCgmCommand c)
        {
            using var pen = _context.CreatePen();
            c.Draw(g, pen);     // chaque commande gère sa propre transformation/traitement
            return true;
        }

        #region Méthodes de transformation et contexte
        private PointF TransformPoint(System.Drawing.Point point)
        {
            // Transformation VDC vers coordonnées écran
            var VdcExtent = CgmContext.VdcExtent;
            var x = (float)((point.X - _context.VdcExtent.Left) * Options.Width / VdcExtent.Width);
            var y = (float)((point.Y - _context.VdcExtent.Top) * Options.Height / VdcExtent.Height);

            return new PointF(x, y);
        }

        // A deplacer dans RenderContext
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
            var modelValue = command.ColourModel;
            var model = (ColorModelEnum)modelValue;
            //var model = modelValue switch
            //{
            //    0 => CgmContext.ColorModelEnum.Indexed,
            //    1 => CgmContext.ColorModelEnum.RGB,
            //    2 => CgmContext.ColorModelEnum.CMYK,
            //    _ => CgmContext.ColorModelEnum.Indexed
            //};
            CgmContext.SetColourModel(model);
        }

        private void UpdateIntegerPrecision(IntegerPrecisionCommand command)
        {
            CgmContext.SetIntegerPrecision(command.GetPrecision());
            //_context.IntegerPrecision = command.GetPrecision();
        }

        private void UpdateVdcExtent(MaximumVdcExtentCommand command)
        {
            CgmContext.SetVdcExtent(command.Point1, command.Point2);
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
