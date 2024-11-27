using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Media;

namespace SandTetris.ViewModels;

public partial class AddEmployeePageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Employee thisEmployee = new Employee { FullName = "", Title = "", DoB = DateTime.Now};

    [ObservableProperty]
    private string departmentID = "";

    public AddEmployeePageViewModel(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        DepartmentID = (string)query["departmentID"];
    }

    private readonly IEmployeeRepository _employeeRepository;

    [RelayCommand]
    async Task Submit()
    {
        if (string.IsNullOrEmpty(ThisEmployee.FullName))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a full name", "OK");
            return;
        }
        if (string.IsNullOrEmpty(ThisEmployee.Title))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a title", "OK");
            return;
        }
        if (string.IsNullOrEmpty(ThisEmployee.Id))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter an employee id", "OK");
            return;
        }

        ThisEmployee.DepartmentId = DepartmentID;

        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "add", ThisEmployee }
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task AddPhoto()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Pick a photo"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    ThisEmployee.Avatar = memoryStream.ToArray();
                    ThisEmployee.AvatarFileExtension = Path.GetExtension(result.FullPath);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}
