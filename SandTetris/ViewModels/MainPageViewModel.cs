using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public MainPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        databasePath = "No database selected";
    }

    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private string databasePath;

    [RelayCommand]
    async Task PickFile()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Please select a .sqlite file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.database" } }, 
                    { DevicePlatform.Android, new[] { "application/x-sqlite3" } },
                    { DevicePlatform.WinUI, new[] { ".sqlite" } },
                    { DevicePlatform.Tizen, new[] { "application/x-sqlite3" } },
                    { DevicePlatform.macOS, new[] { "public.database" } }, 
                })
            });

            if (result != null)
            {
                DatabasePath = result.FullPath;
                await _databaseService.Initialize(DatabasePath);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
