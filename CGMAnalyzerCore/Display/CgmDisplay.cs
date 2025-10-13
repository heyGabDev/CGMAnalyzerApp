using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Geometry;
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
        public List<BaseCgmCommand> Commands { get; private set; } = new();
        public List<string> Messages { get; private set; } = new();
        public TileArrayInfo CurrentTileArrayInfo { get; private set; }
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

        /// <summary>
        /// Configure les informations de tableau de tuiles pour le rendu
        /// </summary>
        public void SetTileArrayInfo(TileArrayInfo tileArrayInfo)
        {
            if (tileArrayInfo == null)
                throw new ArgumentNullException(nameof(tileArrayInfo));

            CurrentTileArrayInfo = tileArrayInfo;

            // Log pour debugging
            LogMessage($"TileArrayInfo configuré : Start={tileArrayInfo.StartPosition}, " +
                                  $"TilesInPath={tileArrayInfo.TilesInPathDirection}, " +
                                  $"CellsPerTile: Path={tileArrayInfo.CellsPerTileInPathDirection}, " +
                                  $"Line={tileArrayInfo.CellsPerTileInLineDirection}");
        }

        /// <summary>
        /// Dessine un tile array si les informations sont disponibles
        /// </summary>
        //public void DrawTileArray(PointF position)
        public void DrawTileArray()
        {
            if (CurrentTileArrayInfo == null || Graphics == null)
                return;
            try
            { // Calculer la position de départ en coordonnées écran
                var startPos = ConvertPoint2DToPointF(CurrentTileArrayInfo.StartPosition);
                var scaledPos = ScalePoint(startPos.X, startPos.Y);

                // Calculer les dimensions des cellules en tenant compte de l'échelle
                var cellWidth = (float)(CurrentTileArrayInfo.TileSizeInPathDirection /
                                        CurrentTileArrayInfo.CellsPerTileInPathDirection * Scale);
                var cellHeight = (float)(CurrentTileArrayInfo.TileSizeInLineDirection /
                                         CurrentTileArrayInfo.CellsPerTileInLineDirection * Scale);

                // Calculer le nombre total de cellules
                int totalCellsInPath = CurrentTileArrayInfo.TilesInPathDirection *
                                       CurrentTileArrayInfo.CellsPerTileInPathDirection;
                int totalCellsInLine = CurrentTileArrayInfo.CellsPerTileInLineDirection;

                // Dessiner la grille
                using (var gridPen = new Pen(Color.LightGray, 1))
                {
                    // Lignes verticales (direction path)
                    for (int i = 0; i <= totalCellsInPath; i++)
                    {
                        float x = scaledPos.X + i * cellWidth;
                        Graphics.DrawLine(gridPen,
                            x, scaledPos.Y,
                            x, scaledPos.Y + totalCellsInLine * cellHeight);
                    }

                    // Lignes horizontales (direction line)
                    for (int j = 0; j <= totalCellsInLine; j++)
                    {
                        float y = scaledPos.Y + j * cellHeight;
                        Graphics.DrawLine(gridPen,
                            scaledPos.X, y,
                            scaledPos.X + totalCellsInPath * cellWidth, y);
                    }
                }

                // Dessiner les tuiles si des données sont disponibles
                if (CurrentTileArrayInfo.TileData != null && CurrentTileArrayInfo.TileData.Length > 0)
                {
                    DrawTileData(scaledPos, cellWidth, cellHeight, totalCellsInPath, totalCellsInLine);
                }

                // Dessiner un contour pour chaque tuile complète
                using (var tilePen = new Pen(Color.DarkGray, 2))
                {
                    var tileWidth = cellWidth * CurrentTileArrayInfo.CellsPerTileInPathDirection;
                    var tileHeight = cellHeight * CurrentTileArrayInfo.CellsPerTileInLineDirection;

                    for (int i = 0; i < CurrentTileArrayInfo.TilesInPathDirection; i++)
                    {
                        var rect = new RectangleF(
                            scaledPos.X + i * tileWidth,
                            scaledPos.Y,
                            tileWidth,
                            tileHeight
                        );
                        Graphics.DrawRectangle(tilePen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Erreur lors du dessin du TileArray : {ex.Message}");
            }
            //try
            //{
            //    // Implémentation basique : dessiner une grille représentant le tile array
            //    var scaledPos = ScalePoint(position.X, position.Y);
            //    var cellWidth = CurrentTileArrayInfo.CellWidth * Scale;
            //    var cellHeight = CurrentTileArrayInfo.CellHeight * Scale;

            //    using (var gridPen = new Pen(Color.LightGray, 1))
            //    {
            //        // Lignes verticales
            //        for (int i = 0; i <= CurrentTileArrayInfo.Width; i++)
            //        {
            //            float x = scaledPos.X + i * cellWidth;
            //            Graphics.DrawLine(gridPen,
            //                x, scaledPos.Y,
            //                x, scaledPos.Y + CurrentTileArrayInfo.Height * cellHeight);
            //        }

            //        // Lignes horizontales
            //        for (int j = 0; j <= CurrentTileArrayInfo.Height; j++)
            //        {
            //            float y = scaledPos.Y + j * cellHeight;
            //            Graphics.DrawLine(gridPen,
            //                scaledPos.X, y,
            //                scaledPos.X + CurrentTileArrayInfo.Width * cellWidth, y);
            //        }
            //    }

            //    // Si des données de tuiles sont disponibles, les dessiner
            //    if (CurrentTileArrayInfo.TileData != null)
            //    {
            //        DrawTileData(scaledPos, cellWidth, cellHeight);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogMessage($"Erreur lors du dessin du TileArray : {ex.Message}");
            //}
        }

        //private void DrawTileData(PointF position, float cellWidth, float cellHeight)
        private void DrawTileData(PointF position, float cellWidth, float cellHeight, int totalCellsInPath, int totalCellsInLine)
        {
            // Dessiner chaque tuile selon ses données
            for (int row = 0; row < CurrentTileArrayInfo.Height; row++)
            {
                for (int col = 0; col < CurrentTileArrayInfo.Width; col++)
                {
                    var tileIndex = row * CurrentTileArrayInfo.Width + col;
                    if (tileIndex >= CurrentTileArrayInfo.TileData.Length)
                        break;

                    var tileValue = CurrentTileArrayInfo.TileData[tileIndex];
                    if (tileValue > 0) // Si la tuile est "active"
                    {
                        var rect = new RectangleF(
                            position.X + col * cellWidth,
                            position.Y + row * cellHeight,
                            cellWidth,
                            cellHeight
                        );

                        using (var brush = new SolidBrush(GetTileColor(tileValue)))
                        {
                            Graphics.FillRectangle(brush, rect);
                        }
                    }
                }
            }
        }

        private Color GetTileColor(int tileValue)
        {
            // Conversion valeur -> couleur
            // Vous pouvez adapter cette logique selon vos besoins
            if (tileValue == 1)
                return CurrentColor; // Utiliser la couleur courante pour les tuiles actives

            // Ou générer une couleur basée sur la valeur
            return Color.FromArgb(255,
                (tileValue * 37) % 256,
                (tileValue * 73) % 256,
                (tileValue * 109) % 256);

        }

        /// <summary>
        /// Convertit un Point2D (CGM) en PointF (GDI+)
        /// </summary>
        private PointF ConvertPoint2DToPointF(Point2D point)
        {
            // Adapter selon votre type Point2D (Integer ou Double)
            if (point is Point2D.Double doublePoint)
            {
                return new PointF((float)doublePoint.X, (float)doublePoint.Y);
            }
            else if (point is Point2D.Double intPoint)
            {
                return new PointF((float)intPoint.X, (float)intPoint.Y);
            }

            // Fallback
            return PointF.Empty;
        }
    }
}
