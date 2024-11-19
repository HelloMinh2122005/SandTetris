using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels.EmployeeViewModel;
// this page will be used to display the employee information (add new and edit, view detail existing employee)

[QueryProperty(nameof(Employee), "EmployeePara")]
[QueryProperty("command", "commnad")]
public partial class EmployeeInfoViewModel : ObservableObject
{
    public EmployeeInfoViewModel()
    {
        EmployeePara = new Employee { FullName = "" , Title = ""};
        command = "";
    }

    [ObservableProperty]
    // this is where the employee should be binded to (for editing existing employee)
    private Employee employeePara;

    [ObservableProperty]
    // this is where the command should be binded to 
    private string command;

    [RelayCommand]
    // this is where the Save button should be binded to
    public async void Save()
    {
        if (command == "Add")
        {
            await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
            {
                {"Add", EmployeePara }
            });
        }
        else
        {
            await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
            {
                {"Update", EmployeePara }
            });
        }
    }

    [RelayCommand]
    // this is where the Cancel button should be binded to
    public async void Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    // this is where the Edit Photo button should be binded to 
    public async void EditPhoto()
    {
        // i'll come back to this later 
        await Task.CompletedTask;
    }
}
