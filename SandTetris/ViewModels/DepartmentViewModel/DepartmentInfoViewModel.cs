using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels.DepartmentViewModel;
// this page will be used to display the department information (add new and edit existing department)

[QueryProperty(nameof(Department), "DepartmentPara")]
[QueryProperty("Command", "Commnad")]
public partial class DepartmentInfoViewModel : ObservableObject
{
    public DepartmentInfoViewModel()
    {
        departmentPara = new Department { Name = "" };
        command = "";
    }


    [ObservableProperty]
    // this is where the department should be binded to (for editing existing department)
    private Department departmentPara;

    private string command;


    [RelayCommand]
    // this is where the Save button should be binded to
    public async void Save()
    {
        if (command == "Add")
        {
            await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
            {
                {"Add", departmentPara } 
            });
        }
        else
        {
            await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
            {
                {"Update", departmentPara }
            });
        }
    }

    [RelayCommand]
    // this is where the Cancel button should be binded to
    public async void Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
