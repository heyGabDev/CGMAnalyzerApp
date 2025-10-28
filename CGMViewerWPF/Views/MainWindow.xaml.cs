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
    }
}