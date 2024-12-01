using Frontend.Controller;
using Frontend.Services;
namespace CHESS_UI_BLAZOR_HYBRID;

public partial class App : Application
{
    private readonly PuzzleManager _puzzleManager;
    private readonly IFileStorageService _fileStorageService;

    public App(PuzzleManager PuzzleManager, IFileStorageService fileStorageService)
    {
        InitializeComponent();
        _puzzleManager = PuzzleManager;
        MainPage = new MainPage();
        _fileStorageService = fileStorageService;
    }
    protected override void OnStart()
    {
        base.OnStart();
        _puzzleManager.Initialize();
    }
    protected override void OnResume()
    {
        base.OnResume();
        _puzzleManager?.Initialize();
    }
    protected override void OnSleep()
    {
        base.OnSleep();
        Task.Run(async () => {
            await _fileStorageService.SaveAsync("savedGames.json", _puzzleManager.GetQueue());
        });
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        Task.Run(async () => {
            await _fileStorageService.SaveAsync("savedGames.json", _puzzleManager.GetQueue());
        });
    }
}