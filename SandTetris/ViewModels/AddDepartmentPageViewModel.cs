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

public partial class AddDepartmentPageViewModel : ObservableObject
{
    [ObservableProperty]
    private Department thisDepartment = new Department{ Id = "", Name = ""};

    [RelayCommand]
    async Task Submit()
    {
        if (string.IsNullOrEmpty(ThisDepartment.Name))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a department name", "OK");
            return;
        }
        if (string.IsNullOrEmpty(ThisDepartment.Id))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a department id", "OK");
            return;
        }
        if (string.IsNullOrEmpty(thisDepartment.HeadOfDepartmentId))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a department head id", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "NewDepartment", ThisDepartment }
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
