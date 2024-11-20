using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Services;
using SandTetris.Views;

namespace SandTetris.ViewModels;

public partial class MainViewModel (DatabaseService dbService) : ObservableObject
{
    [ObservableProperty]
    private string dataFilePath = "";

    [ObservableProperty]
    private bool showLoadingScreen = false;

    // Commented out the InitializeDbAndNavigate method
    /*
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
        await Shell.Current.GoToAsync($"{nameof(DepartmentPage)}");
    }
    */

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

    [RelayCommand]
    private async Task SelectDataFile()
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".db", ".sqlite", ".sqlite3" } },
            { DevicePlatform.Android, new[] { "application/octet-stream" } },
            { DevicePlatform.iOS, new[] { "public.database" } }
        });

        var options = new PickOptions
        {
            PickerTitle = "Select Database File",
            FileTypes = customFileType
        };

        var file = await FilePicker.Default.PickAsync(options);

        if (file == null)
            return;

        DataFilePath = file.FullPath;
    }

    [RelayCommand]
    private async Task UseSelectedDatabase()
    {
        if (string.IsNullOrWhiteSpace(DataFilePath))
        {
            await Shell.Current.DisplayAlert("Error", "Please select a database file.", "OK");
            return;
        }

        await TryInitializeDatabaseAsync(DataFilePath);
        //Preferences.Set("DB_PATH", DataFilePath); // Commented out
        await Shell.Current.GoToAsync($"{nameof(DepartmentPage)}");
    }

    [RelayCommand]
    private async Task UseDefaultDatabase()
    {
        var defaultPath = Path.Combine(FileSystem.AppDataDirectory, "default.db");
        await TryInitializeDatabaseAsync(defaultPath);
        //Preferences.Set("DB_PATH", defaultPath); // Commented out
        await Shell.Current.GoToAsync($"{nameof(DepartmentPage)}");
    }
}