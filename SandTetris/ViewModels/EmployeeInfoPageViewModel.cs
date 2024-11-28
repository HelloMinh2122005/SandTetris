using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class EmployeeInfoPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Employee thisEmployee = new Employee { FullName = "", Title = "" };

    [ObservableProperty]
    private Employee preEmployee = new Employee { FullName = "", Title = "" };

    [ObservableProperty]
    private string employeeID = "";

    [ObservableProperty]
    private bool isVisible = false;

    [ObservableProperty]
    private bool isReadOnly = true;

    [ObservableProperty]
    private bool isHead = false;

    [ObservableProperty]
    private ImageSource avartaImage = ImageSource.FromFile("profile.png");

    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeInfoPageViewModel(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        EmployeeID = (string)query["employeeID"];
        string command = (string)query["command"];
        if (command == "edit")
        {
            IsVisible = true;
            IsReadOnly = false;
        }

        ThisEmployee = await _employeeRepository.GetEmployeeByIdAsync(EmployeeID) ?? new Employee { FullName = "", Title = "" };
        var Head = await _departmentRepository.GetDepartmentHeadAsync(ThisEmployee.DepartmentId);
        if (ThisEmployee.Id == Head.Id)
            IsHead = true;
        AvartaImage = ImageSource.FromStream(() => new MemoryStream(ThisEmployee.Avatar));
    }

    [RelayCommand]
    async Task Save()
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

        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "edit", ThisEmployee }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "delete", ThisEmployee }
        });
    }

    [RelayCommand]
    async Task EditPhoto()
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
                }
                ThisEmployee.AvatarFileExtension = Path.GetExtension(result.FullPath);
                AvartaImage = ImageSource.FromStream(() => new MemoryStream(ThisEmployee.Avatar));
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}
