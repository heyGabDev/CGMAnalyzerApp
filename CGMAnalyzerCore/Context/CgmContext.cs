using System;
using System.Drawing;
using CGMAnalyzerCore.Commands.PictureCommands;
using CGMAnalyzerCore.Enums.Colors;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Geometry;

namespace CGMAnalyzerCore.Context
{
    /// <summary>
    /// Contexte global CGM : conserve l’état de rendu et de parsing (VDC, précisions, couleurs, extents…).
    /// Indépendant des classes de commandes pour éviter les dépendances croisées.
    /// Role : État global du parsing et des attributs CGM
    /// </summary>
    public static class CgmContext
    {
        // --------------------------------------------------------------------
        // VDC / Précisions (pilotés par VDCType, IntegerPrecision, RealPrecision)
        // --------------------------------------------------------------------
        public static VDCTypeEnum VdcType { get; set; } = VDCTypeEnum.Integer;       // Integer ou Real
        public static int VdcIntegerPrecision { get; set; } = 16;                    // bits (8/16/24/32)
        public static VDCRealPrecisionEnum VdcRealPrecision { get; set; } = VDCRealPrecisionEnum.FixedPoint32;

        public static int IntegerPrecision { get; set; } = 16;
        public static int IndexPrecision { get; set; } = 16;
        public static int NamePrecision { get; set; } = 16;

        /// <summary>
        /// Précision "réelle" de la spec CGM (souvent redondant avec <see cref="VdcRealPrecision"/>).
        /// 0 = Fixed32 par défaut (historique).
        /// </summary>
        public static int RealPrecision { get; set; } = 0;

        // --------------------------------------------------------------------
        // Couleurs / Modèle de couleur
        // --------------------------------------------------------------------
        public static ColorModelEnum ColourModel { get; set; } = ColorModelEnum.RGB;
        public static int ColorIndexPrecision { get; set; } = 8;
        public static int ColorPrecision { get; set; } = 8;
        public static int[] MinimumColorValueRGB { get; set; } = new[] { 0, 0, 0 };
        public static int[] MaximumColorValueRGB { get; set; } = new[] { 255, 255, 255 };

        // État courant de tracé/remplissage (par défaut noir, trait fin)
        public static Color StrokeColor { get; set; } = Color.Black;
        public static Color FillColor { get; set; } = Color.Transparent;
        public static float LineWidth { get; set; } = 1f;

        // --------------------------------------------------------------------
        // VDC Extent (boîte englobante logique du dessin CGM)
        // --------------------------------------------------------------------
        public struct VdcExtentRect
        {
            public double Left;
            public double Top;
            public double Right;
            public double Bottom;

            public VdcExtentRect(double left, double top, double right, double bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            /// <summary>Largeur logique (Right - Left).</summary>
            public double Width => Right - Left;

            /// <summary>
            /// Hauteur logique. En CGM l’axe Y croît vers le HAUT.
            /// On expose Height = Top - Bottom (Top &gt; Bottom en coordonnées CGM classiques).
            /// </summary>
            public double Height => Top - Bottom;

            public static VdcExtentRect FromPoints(Point2D.Double p1, Point2D.Double p2)
            {
                var left = Math.Min(p1.X, p2.X);
                var right = Math.Max(p1.X, p2.X);
                var bottom = Math.Min(p1.Y, p2.Y);
                var top = Math.Max(p1.Y, p2.Y);
                return new VdcExtentRect(left, top, right, bottom);
            }
        }

        /// <summary>
        /// Extent VDC courant (utilisé par le renderer pour transformer VDC → pixels).
        /// </summary>
        public static VdcExtentRect VdcExtent { get; private set; } = new VdcExtentRect(0, 1, 1, 0);

        // --------------------------------------------------------------------
        // Divers parsing
        // --------------------------------------------------------------------
        public static int CurrentLayerId { get; set; } = 0;
        public static ColourSelectionModeCommand.ColorSelectionType ColorSelectionMode { get; internal set; }
        public static SpecificationMode EdgeWidthSpecificationMode { get; internal set; }
        public static SpecificationMode LineWidthSpecificationMode { get; internal set; }
        public static SpecificationMode MarkerSizeSpecificationMode { get; internal set; }
        public static DeviceViewportSpecificationModeCommand.DeviceViewportMode DeviceViewportSpecificationMode { get; internal set; }

        // --------------------------------------------------------------------
        // Helpers de mise à jour (appelés par le renderer quand il croise des commandes "contexte")
        // --------------------------------------------------------------------

        /// <summary>Met à jour l’extent VDC à partir de deux points CGM (min/max).</summary>
        public static void SetVdcExtent(Point2D.Double p1, Point2D.Double p2)
            => VdcExtent = VdcExtentRect.FromPoints(p1, p2);

        public static void SetColourModel(ColorModelEnum model) => ColourModel = model;

        public static void SetIntegerPrecision(int bits) => IntegerPrecision = bits;

        public static void SetVdcType(VDCTypeEnum type) => VdcType = type;

        public static void SetVdcRealPrecision(VDCRealPrecisionEnum precision) => VdcRealPrecision = precision;

        /// <summary>
        /// Crée un <see cref="Pen"/> conforme à l’état courant (couleur/épaisseur).
        /// </summary>
        public static Pen CreatePen()
        {
            var width = LineWidth <= 0 ? 1f : LineWidth;
            return new Pen(StrokeColor, width);
        }

        // --------------------------------------------------------------------
        // Reset : appelé en début de parsing / nouveau fichier
        // --------------------------------------------------------------------
        public static void Reset()
        {
            VdcType = VDCTypeEnum.Integer;
            VdcIntegerPrecision = 16;
            VdcRealPrecision = VDCRealPrecisionEnum.FixedPoint32;

            CurrentLayerId = 0;

            IntegerPrecision = 16;
            IndexPrecision = 16;
            NamePrecision = 16;
            RealPrecision = 0;

            ColourModel = ColorModelEnum.RGB;
            ColorIndexPrecision = 8;
            ColorPrecision = 8;
            MinimumColorValueRGB = new[] { 0, 0, 0 };
            MaximumColorValueRGB = new[] { 255, 255, 255 };

            StrokeColor = Color.Black;
            FillColor = Color.Transparent;
            LineWidth = 1f;

            // Extent par défaut [0..1]
            VdcExtent = new VdcExtentRect(0, 1, 1, 0);
        }
    }
}
