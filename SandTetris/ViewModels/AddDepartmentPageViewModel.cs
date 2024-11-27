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

public partial class AddDepartmentPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Department thisDepartment = new Department { Id = "", Name = "" };

    [ObservableProperty]
    private string command = "";

    [ObservableProperty]
    private bool isInvisible = false;

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Command = (string)query["command"];
        if (Command == "edit")
        {
            ThisDepartment = (Department)query["department"];
            IsInvisible = true;
        }
    }

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
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { Command, ThisDepartment }
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task HeadOfDepartmentTapped()
    {
        if (command == "add")
            await Shell.Current.DisplayAlert("Error", "Cannot add Head ID for new department", "OK");
        if (command == "edit")
            await Shell.Current.DisplayAlert("Error", "Cannot modify the Head ID", "OK");
    }
}
