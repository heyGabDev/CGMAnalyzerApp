using CGMAnalyzerCore.Display;
using CGMAnalyzerCore.Messages;
using CGMAnalyzerCore.Modeles;
using CGMAnalyzerCore.Models;
using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGMViewerWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:44350")
        };

        public ImageSource? CgmPreviewImage { get; set; }
        public ObservableCollection<ImportedFile> ImportedFiles { get; } = new();

        [ObservableProperty]
        private ImportedFile? selectedFile;

        [ObservableProperty]
        private BitmapImage? imageSource;

        [ObservableProperty]
        private List<string>? errors;

        [ObservableProperty]
        private ObservableCollection<string> layers = new();

        [ObservableProperty]
        private ObservableCollection<LayerCheckbox> selectedFileLayers = new();

        [RelayCommand]
        public async Task ImportFilesAsync()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "CGM Files (*.cgm)|*.cgm"
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var filePath in dlg.FileNames)
                {
                    using var content = new MultipartFormDataContent();
                    using var stream = File.OpenRead(filePath);
                    content.Add(new StreamContent(stream), "files", Path.GetFileName(filePath));

                    try
                    {
                        var response = await _httpClient.PostAsync("/api/cgm/import", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var results = await response.Content.ReadFromJsonAsync<List<CGMResult>>();
                            
                            foreach(var result in results)
                            {
                                var bmpUrl = new Uri(_httpClient.BaseAddress, result.BmpPath).ToString();
                                LoadImage(bmpUrl);
                                // ImageSource = new BitmapImage(new Uri(bmpUrl));

                                var imported = new ImportedFile
                                {
                                    FileName = Path.GetFileName(result.BmpPath),
                                    BmpPath = result.BmpPath,   
                                    Layers = result.Layers
                                };
                                //imported.FullPath = Path.Combine(@"C:\VSOnline\2_Pro Project\LGM Project\DemoCGMViewerApp\CGMAnalyzer.API\wwwroot\images", Path.GetFileName(result.BmpPath));
                                imported.FullPath = imported.GetFullPath();

                                ImportedFiles.Add(imported);
                                Errors = result.Errors;
                            }                                                  
                        }
                    }
                    catch (Exception ex)
                    {
                        Errors = new List<string> { "Erreur : " + ex.Message };
                    }
                }
            }
        }

        private void LoadImage(string imageUrl)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Upload img immediatly
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                //bitmap.Freeze(); // Prevent problems threading

                ImageSource = bitmap;
            }
            catch (Exception err)
            {
                MessageBox.Show("Erreur lors du chargement du fichier .bmp : " + err.Message);
            }
        }

        partial void OnSelectedFileChanged(ImportedFile value)
        {
            if (value == null || !File.Exists(value.FullPath))
            {
                Debug.WriteLine($"[ERREUR] Fichier introuvable : {value?.FullPath}");
                return;
            }

            try
            {
                var stream = File.OpenRead(value.FullPath);
                var reader = new BinaryReader(stream);

                var parser = new CgmParser();
                parser.Read(reader);

                // Aperçu CGM généré à partir du parser
                CgmPreviewImage = ShowCgmPreviewImage(parser);

                // Mise à jour des couches
                SelectedFileLayers.Clear();
                foreach (var layer in value.Layers)
                {
                    SelectedFileLayers.Add(new LayerCheckbox
                    {
                        Name = layer,
                        IsChecked = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du traitement du fichier CGM : {ex.Message}");
            }
        }

        private ImageSource ShowCgmPreviewImage(CgmParser parser)
        {
            var renderer = new CgmRenderer(parser.Commands);
            var bmp = renderer.Render();

            using var memory = new MemoryStream();
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            CgmPreviewImage = bitmapImage;
            return CgmPreviewImage;
        }
    }
}
