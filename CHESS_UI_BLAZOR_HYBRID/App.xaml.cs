using Microsoft.Maui.Controls;

namespace CHESS_UI_BLAZOR_HYBRID;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}