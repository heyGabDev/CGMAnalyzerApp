using Microsoft.Maui.Controls;
using System.Net.Http.Json;

namespace DemoCGMViewerApp;

public partial class App : Application
{
    public App()
    {
        MainPage = new NavigationPage(new MainPage());
    }
}
