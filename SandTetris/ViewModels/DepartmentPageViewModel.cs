using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class DepartmentPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string searchbar = "";

    [ObservableProperty]    
    private ObservableCollection<Department> departments = new ObservableCollection<Department>();

    public DepartmentPageViewModel(IDepartmentRepository departmentRepository)
    {
        _idepartmentRepository = departmentRepository;
        OnAppearing();
    }

    private async void OnAppearing()
    {
        await LoadDepartments();
    }

    private readonly IDepartmentRepository _idepartmentRepository;

    private Department selectedDepartment = null!;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("add"))
        {
            Departments.Add((Department)query["add"]);
            _idepartmentRepository.AddDepartmentAsync((Department)query["add"]);
        }
        if (query.ContainsKey("edit"))
        {
            var department = (Department)query["edit"];
            var index = Departments.IndexOf(Departments.FirstOrDefault(d => d.Id == department.Id));
            Departments[index] = department;
            _idepartmentRepository.UpdateDepartmentAsync(department);
        }
    }

    private async Task LoadDepartments()
    { 
        var departments = await _idepartmentRepository.GetDepartmentsAsync();
        Departments = new ObservableCollection<Department>(departments);
    }

    [RelayCommand]
    public void OnItemSelected(Department department)
    {
        selectedDepartment = department;
    }

    [RelayCommand]
    async Task Filter()
    {
        // i'll implement this later
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.GoToAsync($"{nameof(AddDepartmentPage)}", new Dictionary<string, object>
        {
            {"command", "add" }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        try
        {
            await _idepartmentRepository.DeleteDepartmentAsync(selectedDepartment);
            Departments.Remove(selectedDepartment);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }        
    }

    [RelayCommand]
    async Task Edit()
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(AddDepartmentPage)}", new Dictionary<string, object>
        {
            {"command", "edit" },
            {"department", new Department
            {
                Id = selectedDepartment.Id,
                Name = selectedDepartment.Name,
                Description = selectedDepartment.Description
            } }
        });
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(EmployeePage)}", new Dictionary<string, object>
        {
            {"departmentID", selectedDepartment.Id }
        });
    }
}
