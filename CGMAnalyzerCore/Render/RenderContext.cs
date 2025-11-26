using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Render;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

namespace CGMAnalyzerCore.Rendering
{
    /// <summary>
    /// Représente le contexte de rendu pour les commandes CGM.
    /// Combine les paramètres de rendu (taille, zoom, anti-aliasing, fond)
    /// et l’état logique du contexte CGM (VDC extent, couleurs, etc.).
    /// Role :Paramètres spécifiques au rendu/affichage
    /// </summary>
    public class RenderContext :IDisposable
    {

        // --------------------------------------------------------------------
        // OPTIONS DE RENDU (propre au viewer)
        // --------------------------------------------------------------------
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;

        public Color BackgroundColor { get; set; } = Color.White;
        public bool AntiAlias { get; set; } = true;

        /// <summary>
        /// Zoom / échelle d’affichage (1.0 = 100%)
        /// </summary>
        public float ZoomFactor { get; set; } = 1.0f;

        /// <summary>
        /// Décalage en pixels (utile pour le panning)
        /// </summary>
        public PointF Offset { get; set; } = PointF.Empty;

        // --------------------------------------------------------------------
        // Lien avec le contexte CGM global
        // --------------------------------------------------------------------
        public CgmContext.VdcExtentRect VdcExtent => CgmContext.VdcExtent;
        private bool _disposed;

        // --------------------------------------------------------------------
        // Constructeur
        // --------------------------------------------------------------------
        public RenderContext() { }

        public RenderContext(int width, int height)
        {
            Width = width;
            Height = height;
        }
        // --------------------------------------------------------------------
        // Réinitialisation du contexte
        // --------------------------------------------------------------------

        /// <summary>
        /// Réinitialise le contexte de rendu avec de nouvelles options.
        /// Configure le Graphics et applique les paramètres.
        /// </summary>
        public void Reset(Graphics graphics, RenderOptions options)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // Appliquer les options de rendu
            Width = options.Width;
            Height = options.Height;
            BackgroundColor = options.BackgroundColor;
            AntiAlias = options.HighQuality;

            // Réinitialiser zoom et offset
            ZoomFactor = 1.0f;
            Offset = PointF.Empty;

            // Configurer le contexte graphique
            ConfigureGraphics(graphics);
        }

        // --------------------------------------------------------------------
        // Méthodes de transformation (VDC -> Device)
        // --------------------------------------------------------------------

        /// <summary>
        /// Convertit un point CGM (VDC) vers les coordonnées écran (pixels).
        /// </summary>
        public PointF TransformPoint2D(Point2D point2d)
        {
            // Calcul en double pour conserver la précision
            double x = (point2d.X - VdcExtent.Left) * Width / VdcExtent.Width;
            double y = (VdcExtent.Top - point2d.Y) * Height / VdcExtent.Height;

            // Application du zoom et du décalage
            x = x * ZoomFactor + Offset.X;
            y = y * ZoomFactor + Offset.Y;

            return new PointF((float)x, (float)y);
        }

        /// <summary>
        /// Transforme une longueur CGM (en VDC) vers les unités écran.
        /// </summary>
        public float TransformLength(double length)
        {
            return (float)(length * Width / VdcExtent.Width * ZoomFactor);
        }

        /// <summary>
        /// Transforme un rectangle VDC vers les coordonnées pixels.
        /// </summary>
        public RectangleF TransformRect(double left, double top, double right, double bottom)
        {
            var p1 = TransformPoint2D(new Point2D.Double(left, top));
            var p2 = TransformPoint2D(new Point2D.Double(right, bottom));
            return RectangleF.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
        }

        // --------------------------------------------------------------------
        // 🔹 Méthodes utilitaires graphiques
        // --------------------------------------------------------------------

        /// <summary>
        /// Prépare le contexte graphique GDI+ (antialias, fond…).
        /// </summary>
        public void ConfigureGraphics(Graphics g)
        {
            g.Clear(BackgroundColor);
            g.SmoothingMode = AntiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
        }

        /// <summary>
        /// Crée un Pen selon le contexte courant.
        /// </summary>
        public Pen CreatePen()
        {
            // Calcul largeur en fonction du mode
            float width;

            if (CgmContext.LineWidthSpecificationMode == SpecificationMode.ABSOLUTE)
            {
                // Mode ABSOLUTE : LineWidth est en unités VDC, pas en pixels
                // On garde la valeur telle quelle car le scale Graphics s'appliquera
                width = CgmContext.LineWidth <= 0 ? 1f : CgmContext.LineWidth;
            }
            else // SCALED
            {
                // Mode SCALED : LineWidth est un facteur (ex: 0.01 = 1% de la largeur de ligne par défaut)
                width = CgmContext.LineWidth <= 0 ? 1f : CgmContext.LineWidth;
            }

            // Limiter pour éviter traits trop fins ou trop épais
            width = Math.Clamp(width, 0.1f, 5.0f);

            return new Pen(CgmContext.StrokeColor, width);
        }

        /// <summary>
        /// Crée une Brush selon la couleur de remplissage courante.
        /// </summary>
        public Brush CreateBrush()
        {
            return new SolidBrush(CgmContext.FillColor);
        }

        // --------------------------------------------------------------------
        // 🔹 Helpers de conversion ou de réinitialisation
        // --------------------------------------------------------------------
        public void ResetView()
        {
            ZoomFactor = 1.0f;
            Offset = PointF.Empty;
        }

        public void UpdateSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Libérer les ressources si nécessaire
                _disposed = true;
            }
        }
    }
}
