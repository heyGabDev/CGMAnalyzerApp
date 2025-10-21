using CGMAnalyzerCore.Commands.PictureCommands;
using CGMAnalyzerCore.Enums.Precision;
using CGMAnalyzerCore.Geometry;
using System;
using System.Diagnostics;
using System.Drawing;
using static CGMAnalyzerCore.Commands.GraphicCommands.Control.VDCTypeCommand;
using static CGMAnalyzerCore.Commands.MetafileCommands.ColorModelCommand;
using static CGMAnalyzerCore.Commands.SpecificationModeExtensions;

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
        public static VDCTypeEnum VdcType { get; set; } = VDCTypeEnum.INTEGER;
        public static int VdcIntegerPrecision { get; set; } = 16;
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

        /// <summary>
        /// Modèle de couleur utilisé (RGB, CMYK, CIELAB, etc.)
        /// </summary>
        public static ColorModelEnum ColorModel { get; set; } = ColorModelEnum.RGB;

        public static int ColorIndexPrecision { get; set; } = 8;
        public static int ColorPrecision { get; set; } = 8;
        public static int MaximumColorIndex { get; set; } = 255;
        public static int[] MinimumColorValueRGB { get; set; } = new[] { 0, 0, 0 };
        public static int[] MaximumColorValueRGB { get; set; } = new[] { 255, 255, 255 };

        // État courant de tracé/remplissage (par défaut noir, trait fin)
        public static Color StrokeColor { get; set; } = Color.Black;
        public static Color FillColor { get; set; } = Color.Transparent;
        public static float LineWidth { get; set; } = 1f;

        #region ===== TABLE DE COULEURS =====

        /// <summary>
        /// Table de couleurs indexées (remplie par ColourTableCommand)
        /// </summary>
        private static Dictionary<int, Color> _colorTable = new Dictionary<int, Color>();

        /// <summary>
        /// Récupère une couleur depuis la table de couleurs par son index
        /// </summary>
        public static Color GetColorFromIndex(int index)
        {
            // Si la couleur existe dans la table, la retourner
            if (_colorTable.ContainsKey(index))
            {
                return _colorTable[index];
            }

            // Sinon, utiliser des couleurs par défaut standard CGM
            // Selon la spec CGM, les 8 premières couleurs sont normalisées
            return index switch
            {
                0 => Color.White,       // Background (souvent blanc)
                1 => Color.Black,       // Foreground (souvent noir)
                2 => Color.Red,
                3 => Color.Green,
                4 => Color.Blue,
                5 => Color.Yellow,
                6 => Color.Magenta,
                7 => Color.Cyan,
                _ => GenerateDefaultColor(index) // Génération automatique pour les autres index
            };
        }

        /// <summary>
        /// Génère une couleur par défaut basée sur l'index (pour les index > 7)
        /// </summary>
        private static Color GenerateDefaultColor(int index)
        {
            // Stratégie 1: Niveau de gris
            int gray = (index * 255) / Math.Max(CgmContext.MaximumColorIndex, 255);
            return Color.FromArgb(gray, gray, gray);

            // Stratégie 2 (alternative): Couleurs distinctes en utilisant HSV
            // float hue = (index * 137.5f) % 360; // Golden angle pour distribution uniforme
            // return ColorFromHSV(hue, 1.0f, 1.0f);
        }

        /// <summary>
        /// Définit une couleur dans la table de couleurs (utilisé par ColourTableCommand)
        /// </summary>
        public static void SetColorInTable(int index, Color color)
        {
            _colorTable[index] = color;
            Debug.WriteLine($"[CgmContext] ColorTable[{index}] = R:{color.R}, G:{color.G}, B:{color.B}");
        }

        /// <summary>
        /// Efface la table de couleurs (appelé au début d'un nouveau métafichier)
        /// </summary>
        public static void ClearColorTable()
        {
            _colorTable.Clear();
            Debug.WriteLine("[CgmContext] Color table cleared");
        }

        /// <summary>
        /// Retourne le nombre d'entrées dans la table de couleurs
        /// </summary>
        public static int GetColorTableSize()
        {
            return _colorTable.Count;
        }

        #endregion

        #region ===== VDC Extent (boîte englobante logique du dessin CGM)  =====
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
            /// Hauteur logique. En CGM l'axe Y croît vers le HAUT.
            /// On expose Height = Top - Bottom (Top > Bottom en coordonnées CGM classiques).
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
        #endregion

        #region ===== Modes de spécification et autres paramètres  =====
        public static int CurrentLayerId { get; set; } = 0;
        public static ColorSelectionModeCommand.ColorSelectionType ColorSelectionMode { get; set; }
        public static SpecificationMode EdgeWidthSpecificationMode { get; set; }
        public static SpecificationMode LineWidthSpecificationMode { get; set; }
        public static SpecificationMode MarkerSizeSpecificationMode { get; set; }
        public static DeviceViewportSpecificationModeCommand.DeviceViewportMode DeviceViewportSpecificationMode { get; set; }

        // Escape command data
        public static int LastEscapeIdentifier { get; set; }
        public static string LastEscapeDataRecord { get; set; } = "";
        public static Color BackgroundColor { get; internal set; }
        public static SpecificationMode InteriorStyleSpecificationMode { get; internal set; }
        #endregion

        #region ===== Helpers de mise à jour (appelés par le renderer quand il croise des commandes "contexte")  =====
        /// <summary>
        /// Met à jour l'extent VDC à partir de deux points CGM (min/max).
        /// </summary>
        public static void SetVdcExtent(Point2D.Double p1, Point2D.Double p2)
            => VdcExtent = VdcExtentRect.FromPoints(p1, p2);

        /// <summary>
        /// Définit le modèle de couleur utilisé.
        /// Méthode helper pour cohérence avec le reste de l'API.
        /// </summary>
        public static void SetColorModel(ColorModelEnum model)
        {
            ColorModel = model;
        }

        public static void SetIntegerPrecision(int bits) => IntegerPrecision = bits;

        public static void SetVdcType(VDCTypeEnum type) => VdcType = type;

        public static void SetVdcRealPrecision(VDCRealPrecisionEnum precision) => VdcRealPrecision = precision;

        /// <summary>
        /// Crée un <see cref="Pen"/> conforme à l'état courant (couleur/épaisseur).
        /// </summary>
        public static Pen CreatePen()
        {
            var width = LineWidth <= 0 ? 1f : LineWidth;
            return new Pen(StrokeColor, width);
        }
        #endregion

        #region ===== Reset : appelé en début de parsing / nouveau fichier  =====
        public static void Reset()
        {
            VdcType = VDCTypeEnum.INTEGER;
            VdcIntegerPrecision = 16;
            VdcRealPrecision = VDCRealPrecisionEnum.FixedPoint32;

            CurrentLayerId = 0;

            IntegerPrecision = 16;
            IndexPrecision = 16;
            NamePrecision = 16;
            RealPrecision = 0;

            ColorModel = ColorModelEnum.RGB;
            ColorIndexPrecision = 8;
            ColorPrecision = 8;
            MaximumColorIndex = 255;
            MinimumColorValueRGB = new[] { 0, 0, 0 };
            MaximumColorValueRGB = new[] { 255, 255, 255 };

            StrokeColor = Color.Black;
            FillColor = Color.Transparent;
            LineWidth = 1f;

            // Extent par défaut [0..1]
            VdcExtent = new VdcExtentRect(0, 1, 1, 0);

            // Reset des modes
            ColorSelectionMode = ColorSelectionModeCommand.ColorSelectionType.INDEXED;
            EdgeWidthSpecificationMode = SpecificationMode.ABSOLUTE;
            LineWidthSpecificationMode = SpecificationMode.ABSOLUTE;
            MarkerSizeSpecificationMode = SpecificationMode.ABSOLUTE;

            LastEscapeIdentifier = 0;
            LastEscapeDataRecord = "";
        }
        #endregion
    }
}
