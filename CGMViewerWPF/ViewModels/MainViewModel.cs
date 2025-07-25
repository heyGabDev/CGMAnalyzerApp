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

                                // debug
                                //Debug.WriteLine($"CGM Layers : {result.Layers}");

                                // management layers = add 5 layers
                                //Layers.Clear();
                                //foreach (var layer in result.Layers)
                                //{
                                //    Layers.Add(layer); 
                                //}
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


        //partial void OnSelectedFileChanged(ImportedFile value)
        //{
        //    if (value == null)
        //        return;

        //    if (!File.Exists(value.FullPath))
        //    {
        //        Debug.WriteLine($"[ERREUR] Image introuvable à {value.FullPath}");
        //        return;
        //    }

        //    // test display preview
        //    var parser = new CgmParser();

        //    var stream = File.OpenRead(value.FullPath);
        //    var reader = new BinaryReader(stream);
        //    parser.Read(reader);

        //    ShowCgmPreviewImage(parser);

        //    //try
        //    //{
        //    //    // 1. Lire le CGM brut
        //    //    var stream = File.OpenRead(value.FullPath);
        //    //    var reader = new BinaryReader(stream);

        //    //    // 2. Parser avec CgmDisplay
        //    //    var display = new CgmDisplay();
        //    //    display.Read(reader);
        //    //    var commands = display.Commands;

        //    //    // 3. Rendu bitmap
        //    //    var renderer = new CgmRenderer(commands);
        //    //    var bmp = renderer.Render();

        //    //    // 4. Export .bmp
        //    //    var outputDir = Path.Combine(Path.GetDirectoryName(value.FileName)!, "output");
        //    //    Directory.CreateDirectory(outputDir);

        //    //    var bmpFileName = Path.GetFileNameWithoutExtension(value.FileName) + ".bmp";
        //    //    var bmpPath = Path.Combine(outputDir, bmpFileName);

        //    //    bmp.Save(bmpPath);
        //    //    value.BmpPath = bmpPath;
        //    //    //file.Layers = ExtractLayers(commands); // méthode perso à écrire

        //    //    // On peut aussi extraire les layers ici si ton format les supporte
        //    //    value.Layers = commands
        //    //        .Where(cmd => cmd.ElementClass == 5) // par exemple
        //    //        .Select(cmd => cmd.ToString())       // ou cmd.LayerName ?
        //    //        .ToList();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Gestion d'erreur simple
        //    //    Console.WriteLine("Erreur de rendu : " + ex.Message);
        //    //}

        //}

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
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

            //var renderer = new CgmRenderer(parser.Commands);
            //using var bitmap = renderer.Render();

            //using var memory = new MemoryStream();
            //bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            //memory.Position = 0;

            //var image = new BitmapImage();
            //image.BeginInit();
            //image.StreamSource = memory;
            //image.CacheOption = BitmapCacheOption.OnLoad;
            //image.EndInit();

            //ImageSource = image;
        }


        //Messages.Add("Fichier CGM chargé avec succès.");

        //// 1. Lire le fichier CGM brut
        //byte[] cgmBytes = File.ReadAllBytes(url);

        //// 2. Parser le contenu avec ton CgmParser
        //var parser = new CgmParser();
        //parser.Read(new MemoryStream(cgmBytes));

        //// 3. Créer un Bitmap pour le rendu
        //using var bmp = new Bitmap(800, 600);
        //using var g = Graphics.FromImage(bmp);

        //// 4. Créer ton Display avec ce Graphics
        //var display = new CgmDisplay(g);

        //// 5. Rejouer les commandes CGM sur le display
        //foreach (var command in parser.Commands)
        //{
        //    command.Draw(display); // <-- méthode polymorphe à implémenter dans tes commandes
        //}

        //// 6. Stocker/afficher l'image dans ton ViewModel (ex. ImagePreview ou autre)
        //DisplayedBitmap = BitmapToImageSource(bmp); // méthode à écrire si elle n'existe pas



        //partial void OnSelectedFileChanged(ImportedFile value)
        //{
        //    if (value != null)
        //    {
        //        var url = new Uri(_httpClient.BaseAddress, value.BmpPath).ToString();
        //        ImageSource = new BitmapImage(new Uri(url));

        //        SelectedFileLayers.Clear();
        //        foreach (var layer in value.Layers)
        //        {
        //            SelectedFileLayers.Add(new LayerCheckbox { Name = layer, IsChecked = true });
        //        }

        //        // 🚀 Appel de CgmDisplay pour parser le fichier sélectionné
        //        try
        //        {
        //            var fullPath = Path.Combine("TonRépertoireTempCGM", Path.GetFileNameWithoutExtension(value.BmpPath) + ".cgm");
        //            if (File.Exists(fullPath))
        //            {
        //                var display = new CgmDisplay();
        //                display.LoadFromFile(fullPath);

        //                // ⬇️ Tu peux maintenant inspecter les commandes graphiques, etc.
        //                var commandes = display.GetParsedCommands();
        //                Debug.WriteLine($"Nombre de commandes CGM : {commandes.Count}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Erreur lors du parsing CGM : {ex.Message}");
        //        }
        //    }
        //}
    }
}
