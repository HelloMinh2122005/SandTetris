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

namespace SandTetris.ViewModels.EmployeeViewModel;
// this page will be used to display the list of employees

[QueryProperty("DepartmentId", "DepartmentId")]
public partial class EmployeeListViewModel : ObservableObject, IQueryAttributable
{
    public EmployeeListViewModel(IEmployeeRepository employeeRepository)
    {
        Employees = new ObservableCollection<Employee>();
        Searchbar = "";
        SelectedEmployees = new List<Employee>();
        DepartmentId = "";
        _iEmployeeRepo = employeeRepository;
        LoadEmployees();
    }

    [ObservableProperty]
    // this is where the emplyees list should be binded to
    private ObservableCollection<Employee> employees;

    [ObservableProperty]
    // this is where the search bar should be binded to 
    private string searchbar;

    private List<Employee> SelectedEmployees;

    private readonly IEmployeeRepository _iEmployeeRepo;

    public string DepartmentId { get; set; }

    private async void LoadEmployees()
    {
        var employees = await _iEmployeeRepo.GetEmployeesByDepartmentAsync(DepartmentId);
        foreach (var employee in employees)
        {
            Employees.Add(employee);
        }
    }

    async void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("Add"))
        {
            var newEmployee = (Employee)query["Add"];
            Employees.Add(newEmployee);
            await _iEmployeeRepo.AddEmployeeAsync(newEmployee);
        }
        else if (query.ContainsKey("Update"))
        {
            var updatedEmployee = (Employee)query["Update"];
            var index = Employees.IndexOf(updatedEmployee);
            Employees[index] = updatedEmployee;
            await _iEmployeeRepo.UpdateEmployeeAsync(updatedEmployee);
        }
    }

    [RelayCommand]
    // this is where the add button should be binded to 
    async Task Add()
    {
        await Shell.Current.GoToAsync($"EmployeeInfoPage", new Dictionary<string, object>
        {
            {"Command", "Add" }
        });
    }

    [RelayCommand]
    // this is where the delete button should be binded to 
    async Task Del()
    {
        if (SelectedEmployees.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No employee selected", "OK");
            return;
        }
        foreach (var employee in SelectedEmployees.ToList())
        {
            await _iEmployeeRepo.DeleteEmployeeAsync(employee);
            Employees.Remove(employee);
        }
        SelectedEmployees.Clear();
    }

    [RelayCommand]
    // this is where the edit button should be binded to 
    async Task Edit()
    {
        if (SelectedEmployees.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No employee selected", "OK");
            return;
        }
        if (SelectedEmployees.Count > 1)
        {
            await Shell.Current.DisplayAlert("Error", "Multiple employees selected", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"EmployeeInfoPage", new Dictionary<string, object>
        {
            {"EmployeePara", SelectedEmployees.First() },
            {"Command", "Edit" }
        });
    }

    [RelayCommand]
    // this is where the detail button should be binded to 
    async Task Detail(string employeeId)
    {
        if (SelectedEmployees.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No employee selected", "OK");
            return;
        }
        if (SelectedEmployees.Count > 1)
        {
            await Shell.Current.DisplayAlert("Error", "Multiple employees selected", "OK");
            return;
        }
        var employee = await _iEmployeeRepo.GetEmployeeByIdAsync(employeeId);
        if ( employee == null)
        {
            await Shell.Current.DisplayAlert("Error", "No employee found", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"EmployeeInfoPage", new Dictionary<string, object>
        {
            {"EmployeePara", employee },
            {"Command", "Detail" }
        });
    }

    [RelayCommand]
    // this is where the search button should be binded to
    async Task Search()
    {
        Employees.Clear();
        if (string.IsNullOrEmpty(Searchbar))
        {
            LoadEmployees();
            return;
        }
        var employees = await _iEmployeeRepo.GetEmployeesAsync();
        foreach (var employee in employees)
        {
            // i'll come back to this later, afer having full info from the UI 
            if (employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase))
            {
                Employees.Add(employee);
            }
        }
    }

    [RelayCommand]
    // this is where the Filter button should be binded t
    async Task Filter()
    {
        await Task.CompletedTask;
    }
}
