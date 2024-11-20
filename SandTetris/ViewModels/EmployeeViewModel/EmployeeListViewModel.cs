using CommunityToolkit.Mvvm.ComponentModel;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels.EmployeeViewModel;

public partial class EmployeeListViewModel : ObservableObject, IQueryAttributable
{
    public EmployeeListViewModel(IEmployeeRepository employeeRepository)
    {
        Employees = new ObservableCollection<Employee>();
        Searchbar = "";
        _iEmployeeRepo = employeeRepository;
        LoadEmployees();
    }

    [ObservableProperty]
    private ObservableCollection<Employee> employees;

    [ObservableProperty]
    private string searchbar;

    private List<Employee> SelectedEmployees;
    private readonly IEmployeeRepository _iEmployeeRepo;

    private async void LoadEmployees()
    {
        var employees = await _iEmployeeRepo.GetEmployeesAsync();
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
}
