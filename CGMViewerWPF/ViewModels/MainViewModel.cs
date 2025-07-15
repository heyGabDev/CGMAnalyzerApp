using CGMAnalyzerCore.Modeles;
using CGMAnalyzerCore.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CGMViewerWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:44350")
        };

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
                                // debug
                                Debug.WriteLine($"CGM Layers : {result.Layers}");

                                Errors = result.Errors;
                                ImportedFiles.Add(imported);    

                                // management layers = add 5 layers
                                Layers.Clear();
                                foreach (var layer in result.Layers)
                                {
                                    Layers.Add(layer); 
                                }
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
            if (value != null)
            {
                var url = new Uri(_httpClient.BaseAddress, value.BmpPath).ToString();
                ImageSource = new BitmapImage(new Uri(url));

                SelectedFileLayers.Clear();
                foreach (var layer in value.Layers)
                {
                    SelectedFileLayers.Add(new LayerCheckbox { Name = layer, IsChecked = true });
                }
            }
        }
    }
}
