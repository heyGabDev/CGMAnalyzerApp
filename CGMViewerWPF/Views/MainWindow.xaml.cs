using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using CGMAnalyzerCore.Test;
using CGMViewerWPF.ViewModels;
using System.IO;
using System.Windows;
using System.Drawing.Imaging;


namespace CGMViewerWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void BtnCreateTestFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Créer le fichier de test
                string testPath = Path.Combine(Path.GetTempPath(), "test_triangle.cgm");
                CgmTestFileGenerator.CreateSimplePolygonCGM(testPath);

                // 2. Parser le fichier
                var parser = new CgmParser();
                using var fs = File.OpenRead(testPath);
                parser.Load(fs, "test_triangle.cgm");

                // 3. Vérifier les polygones
                var polygon = parser.Commands.FirstOrDefault(c =>
                    c.ElementClass == 4 && c.ElementId == 7);

                if (polygon != null)
                {
                    // 4. Polygon avec CgmRenderer
                    var renderer = new CgmRenderer(parser.Commands);
                    var bitmap = renderer.Render();

                    // 5. Sauvegarde img
                    string imgPath = Path.Combine(Path.GetTempPath(), "test_triangle.png");
                    bitmap.Save(imgPath, ImageFormat.Png);

                    // 6. Affichage result
                    var result = MessageBox.Show(
                        $"✅ SUCCÈS !\n\n" +
                        $"Polygon détecté avec succès\n" +
                        $"Total commandes parsées : {parser.Commands.Count}\n" +
                        $"Fichier : {testPath}\n\n" +
                        $"Voulez-vous l'ouvrir dans le viewer ?",
                        "Test réussi",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = imgPath,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"❌ ÉCHEC\n\n" +
                        $"Aucun polygon détecté\n" +
                        $"Total commandes : {parser.Commands.Count}\n\n" +
                        $"Messages :\n{string.Join("\n", parser.Messages)}",
                        "Test échoué",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERREUR\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}