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
    private string employeeID = "";

    [ObservableProperty]
    private bool isVisible = false;

    [ObservableProperty]
    private bool isReadOnly = true;

    public EmployeeInfoPageViewModel(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
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
    }

    private readonly IEmployeeRepository _employeeRepository;

    [RelayCommand]
    async Task Save()
    {
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
}
