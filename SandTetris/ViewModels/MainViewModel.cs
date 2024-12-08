using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Services;
using SandTetris.Views;

namespace SandTetris.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string dataFilePath = "";

    [ObservableProperty]
    private bool showLoadingScreen = false;

    private DatabaseService dbService;

    public MainViewModel(DatabaseService databaseService)
    {
        dbService = databaseService;
        OnAppearing();
    }

    public async void OnAppearing()
    {
        await UseDefaultDatabase();
    }

    // Commented out the InitializeDbAndNavigate method
    public async Task InitializeDbAndNavigate()
    {
        var dbPath = Preferences.Get("DB_PATH", "");
        if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
        {
            // Prompt user to select a database or use default
            return;
        }

        ShowLoadingScreen = true;

        await TryInitializeDatabaseAsync(dbPath);
    }

    private async Task TryInitializeDatabaseAsync(string dbPath)
    {
        try
        {
            await dbService.Initialize(dbPath);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task UseDefaultDatabase()
    {
        var defaultPath = Path.Combine(FileSystem.AppDataDirectory, "default.db");
        await TryInitializeDatabaseAsync(defaultPath);
        Preferences.Set("DB_PATH", defaultPath); // Commented out
    }
}