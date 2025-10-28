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

        // ProgressBar
        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int progressValue;

        [ObservableProperty]
        private string progressText = "";

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
                IsLoading = true; ProgressValue = 0; ProgressText = "Import des fichiers…";

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
                                LoadImage(bmpUrl); // afficher l’aperçu BMP dans l’UI

                                var imported = new ImportedFile
                                {
                                    FileName = Path.GetFileName(result.BmpPath),
                                    BmpPath = result.BmpPath,   
                                    Layers = result.Layers
                                };
                                //imported.FullPath = Path.Combine(@"C:\VSOnline\2_Pro Project\LGM Project\DemoCGMViewerApp\CGMAnalyzer.API\wwwroot\images", Path.GetFileName(result.BmpPath));
                                imported.FullPath = filePath;

                                ImportedFiles.Add(imported);
                                Errors = result.Errors;

                                // Déclenche l’aperçu + la barre
                                SelectedFile = imported;
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

        [RelayCommand]
        private void ClearImportedFiles()
        {
            ImportedFiles.Clear();
            SelectedFile = null;
            ImageSource = null;
            Errors = null;
        }

        [RelayCommand]
        private void RemoveSelectedFile()
        {
            if (SelectedFile is null) return;
            ImportedFiles.Remove(SelectedFile);
            SelectedFile = null;
            ImageSource = null;
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

        partial void OnSelectedFileChanged(ImportedFile? value)
        {
            if (value == null || !File.Exists(value.FullPath))
            {
                Debug.WriteLine($"[ERREUR] Fichier introuvable : {value?.FullPath}");
                return;
            }

            _ = LoadCgmFileAsync(value);

            //try
            //{
            //    //DEBUG
            //    //Debug.WriteLine($"[OnSelectedFileChanged] Loading file: {value.FullPath}");

            //    using var stream = File.OpenRead(value.FullPath);
            //    var parser = new CgmParser();
            //    parser.Load(stream, value.FileName);

            //    // DEBUG
            //    //Debug.WriteLine($"[OnSelectedFileChanged] Parsed {parser.Commands.Count} commands");
            //    //Debug.WriteLine($"[OnSelectedFileChanged] Messages: {string.Join(", ", parser.Messages)}");

            //    // Vérifier s'il y a des commandes graphiques
            //    //var graphicalCommands = parser.Commands.Where(c => c.ElementClass == 4).ToList();
            //    //Debug.WriteLine($"[OnSelectedFileChanged] Graphical commands: {graphicalCommands.Count}");

            //    //foreach (var cmd in graphicalCommands.Take(5))
            //    //{
            //    //    Debug.WriteLine($"[OnSelectedFileChanged] - {cmd.GetType().Name} (EC={cmd.ElementClass}, EID={cmd.ElementId})");
            //    //}

            //    // Aperçu CGM généré à partir du parser
            //    CgmPreviewImage = ShowCgmPreviewImage(parser);

            //    // Mise à jour des couches
            //    SelectedFileLayers.Clear();
            //    foreach (var layer in value.Layers)
            //    {
            //        SelectedFileLayers.Add(new LayerCheckbox
            //        {
            //            Name = layer,
            //            IsChecked = true
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Erreur lors du traitement du fichier CGM : {ex.Message}");
            //}
        }

        private async Task LoadCgmFileAsync(ImportedFile file)
        {
            IsLoading = true;
            ProgressValue = 0;
            ProgressText = "Chargement du fichier...";

            try
            {
                await Task.Run(() =>
                {
                    // 1. Parsing
                    UpdateProgress(10, "Parsing du CGM...");

                    using var stream = File.OpenRead(file.FullPath);
                    var parser = new CgmParser();
                    parser.Load(stream, file.FileName);

                    UpdateProgress(50, $"Parsed {parser.Commands.Count} commandes");

                    // 2. Rendu
                    UpdateProgress(60, "Génération de l'image...");

                    CgmPreviewImage = ShowCgmPreviewImage(parser);

                    UpdateProgress(90, "Finalisation...");

                    // 3. Layers
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SelectedFileLayers.Clear();
                        foreach (var layer in file.Layers)
                        {
                            SelectedFileLayers.Add(new LayerCheckbox
                            {
                                Name = layer,
                                IsChecked = true
                            });
                        }
                    });

                    UpdateProgress(100, "Terminé !");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
                ProgressText = $"Erreur : {ex.Message}";
            }
            finally
            {
                // Masquer après 500ms
                await Task.Delay(500);
                IsLoading = false;
            }
        }

        private void UpdateProgress(int value, string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgressValue = value;
                ProgressText = text;
            });
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
