using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class SelectHeadOfDepartmentPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

    [ObservableProperty]
    private string searchbar = "";

    public SelectHeadOfDepartmentPageViewModel(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentID = (string)query["departmentID"];
        await LoadEmployeeOnDepartmentID();
    }

    private string departmentID = "";
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private Employee selectedEmployee = new Employee { FullName = "", Title = "" };

    async Task LoadEmployeeOnDepartmentID()
    {
        var employeeList = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentID);
        Employees = new ObservableCollection<Employee>(employeeList);
    }

    [RelayCommand]
    void ItemSelected(Employee employee)
    {
        if (employee == null)
            return;
        selectedEmployee = employee;
    }

    [RelayCommand]
    async Task Search()
    {
        var employeeList = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentID);
        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            employeeList = employeeList.Where(e => e.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase));
        }
        Employees = new ObservableCollection<Employee>(employeeList);
    }

    [RelayCommand]
    async Task Save()
    {
        if (selectedEmployee == null || string.IsNullOrEmpty(selectedEmployee.FullName))
        {
            await Shell.Current.DisplayAlert("Error", "Please select a head of department", "OK");
            return;
        }
        await _departmentRepository.UpdateDeparmentHeadAsync(departmentID, selectedEmployee.Id);
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "newHeadID", selectedEmployee.Id } 
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
