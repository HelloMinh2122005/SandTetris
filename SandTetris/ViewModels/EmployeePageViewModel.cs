using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class EmployeePageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

    [ObservableProperty]
    private string searchbar = "";  

    private Employee selectedEmployee = null!;

    private string departmentID = "";

    private readonly IEmployeeRepository _employeeRepository;

    public EmployeePageViewModel(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("departmentID"))
        {
            departmentID = (string)query["departmentID"];
            LoadEmployeeOnDepartmentID();
        }
    }

    async void LoadEmployeeOnDepartmentID()
    {
        var employeeList = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentID);
        employees = new ObservableCollection<Employee>(employeeList);
    }

    [RelayCommand]
    void IntemSelected(Employee employee)
    {
        if (employee == null) return;
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
        Employees = (ObservableCollection<Employee>)employeeList;
    }

    [RelayCommand]
    async Task Add()
    {
        /*
        await Shell.Current.GoToAsync($"{nameof(AddEmployeePage)}", new Dictionary<string, object>
        {
            {"command", "add" },
            {"departmentID", departmentID }
        });
        */
    }

    [RelayCommand]
    async Task Delete()
    {
        if (selectedEmployee == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }
        try
        {
            await _employeeRepository.DeleteEmployeeAsync(selectedEmployee);
            Employees.Remove(selectedEmployee);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task Edit()
    {
        if (selectedEmployee == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }
        /*
        await Shell.Current.GoToAsync($"{nameof(AddEmployeePage)}", new Dictionary<string, object>
        {
            {"command", "edit" },
            {"employee", selectedEmployee }
        });
        */
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedEmployee == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }

        /*
        await Shell.Current.GoToAsync($"{nameof(EmployeeDetailPage)}", new Dictionary<string, object>
        {
            {"employeeID", selectedEmployee.Id }
        });
        */
    }
}
