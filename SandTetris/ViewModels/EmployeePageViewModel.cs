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
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("departmentID"))
        {
            departmentID = (string)query["departmentID"];
            query.Remove("departmentID");
            await LoadEmployeeOnDepartmentID();
        }
        if (query.ContainsKey("add"))
        {
            var newEmployee = (Employee)query["add"];
            query.Remove("add");
            Employees.Add(newEmployee);

            await _employeeRepository.AddEmployeeAsync(newEmployee);

            using var stream = new MemoryStream(newEmployee.Avatar);
            await _employeeRepository.UploadAvatarAsync(newEmployee.Id, stream, newEmployee.AvatarFileExtension);
        }
        if (query.ContainsKey("delete"))
        {
            var employeeToDelete = (Employee)query["delete"];
            query.Remove("delete");
            await _employeeRepository.DeleteEmployeeAsync(employeeToDelete);
            Employees.Remove(employeeToDelete);
        }
        if (query.ContainsKey("edit"))
        {
            var updatedEmployee = (Employee)query["edit"];
            query.Remove("edit");
            var existingEmployee = Employees.FirstOrDefault(e => e.Id == updatedEmployee.Id);
            if (existingEmployee != null)
            {
                var index = Employees.IndexOf(existingEmployee);
                Employees[index] = updatedEmployee;
                await _employeeRepository.UpdateEmployeeAsync(updatedEmployee);
                using var stream = new MemoryStream(updatedEmployee.Avatar);
                await _employeeRepository.UploadAvatarAsync(updatedEmployee.Id, stream, updatedEmployee.AvatarFileExtension);
            }
        }
    }

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
            employeeList = employeeList.Where(e => 
                e.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
                || e.Id.Contains(Searchbar, StringComparison.OrdinalIgnoreCase));
        }
        Employees.Clear();
        foreach (var employee in employeeList)
        {
            Employees.Add(employee);
        }
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEmployeePage)}", new Dictionary<string, object>
        {
            {"departmentID", departmentID }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        if (selectedEmployee == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }

        var accepted = await Shell.Current.DisplayAlert(
            "Confirm delete department",
            $"Are you sure you want to delete employee {selectedEmployee.FullName}?",
            "Delete", "Cancel"
        );
        if (!accepted) return;

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
        await Shell.Current.GoToAsync($"{nameof(EmployeeInfoPage)}", new Dictionary<string, object>
        {
            {"command", "edit" },
            {"employeeID", selectedEmployee.Id }
        });
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedEmployee == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(EmployeeInfoPage)}", new Dictionary<string, object>
        {
            {"command", "detail" },
            {"employeeID", selectedEmployee.Id }
        });
    }
}
