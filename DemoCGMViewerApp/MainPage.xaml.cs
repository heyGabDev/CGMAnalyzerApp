using DemoCGMViewerApp.Modeles;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Net.Http.Json;

namespace DemoCGMViewerApp;

public partial class MainPage : ContentPage
{
    private Image _cgmImage;
    private CollectionView _errorList;
    private Label _statusLabel; // management error msg step by step
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:44350")
    };

    public MainPage()
    {
        _cgmImage = new Image { HeightRequest = 300 };
        _errorList = new CollectionView();
        _statusLabel = new Label { TextColor = Colors.Red, FontAttributes = FontAttributes.Italic };

        var importButton = new Button { Text = "Importer fichiers CGM" };
        importButton.Clicked += OnPickFolderClicked;

        var layout = new VerticalStackLayout
        {
            Padding = 20,
            Children =
            {
                importButton,
                _statusLabel,
                _cgmImage,
                new Label { Text = "Erreurs S1000D", FontAttributes = FontAttributes.Bold },
                _errorList
            }
        };
        Content = layout;
    }

    private async void OnPickFolderClicked(object sender, EventArgs e)
    {
        try // Errors management catch exception
        {
            var folderPicker = await FilePicker.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Sélectionnez vos fichiers CGM"
            });

            foreach (var file in folderPicker)
            {
                using var content = new MultipartFormDataContent();
                var stream = await file.OpenReadAsync();
                content.Add(new StreamContent(stream), "files", file.FileName);

                _statusLabel.Text = $"Envoi de {file.FileName}...";

                await Task.Delay(1000);

                var response = await _httpClient.PostAsync("/api/cgm/import", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CGMResult>();
                    LoadImage(result.BmpPath);
                    ShowErrors(result.Errors);
                    _statusLabel.Text = $"Traitement réussi pour {file.FileName}";
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    _statusLabel.Text = $"Erreur API ({response.StatusCode}) : {error}";
                    Debug.WriteLine("API Error: " + error); // write logs in console -> details errors
                }
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "Erreur : " + ex.Message;
            Debug.WriteLine("Exception: " + ex);
        }
    }

    private void LoadImage(string path)
    {
        _cgmImage.Source = ImageSource.FromFile(path);
    }

    private void ShowErrors(List<string> errors)
    {
        _errorList.ItemsSource = errors;
    }
}
