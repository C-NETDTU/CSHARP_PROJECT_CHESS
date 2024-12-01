using Frontend.Controller;
using Frontend.Services;
namespace CHESS_UI_BLAZOR_HYBRID;

public partial class App : Application
{
    private readonly PuzzleManager _puzzleManager;
    private readonly IFileStorageService _fileStorageService;

    public App(PuzzleManager PuzzleManager)
    {
        InitializeComponent();
        _puzzleManager = PuzzleManager;
        MainPage = new MainPage();
    }
    protected override void OnStart()
    {
        base.OnStart();
        _puzzleManager.Initialize();
    }
    protected override void OnSleep()
    {
        base.OnSleep();
        _fileStorageService.SaveAsync("savedGames.json", _puzzleManager.GetQueue);
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        _fileStorageService.SaveAsync("savedGames.json",_puzzleManager.GetQueue);
    }
}