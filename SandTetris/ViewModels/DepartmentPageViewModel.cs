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
        LoadDepartments().ConfigureAwait(false);
    }

    private readonly IDepartmentRepository _idepartmentRepository;

    private Department selectedDepartment = new Department { Name = "X" };

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("NewDepartment"))
        {
            Departments.Add((Department)query["NewDepartment"]);
            _idepartmentRepository.AddDepartmentAsync((Department)query["NewDepartment"]);
        }
    }

    private async Task LoadDepartments()
    { 
        var departments = await _idepartmentRepository.GetDepartmentsAsync();
        Departments = new ObservableCollection<Department>(departments);
    }

    [RelayCommand]
    private void ItemSelected(Department department)
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
        await Shell.Current.GoToAsync(nameof(AddDepartmentPage));
    }

    [RelayCommand]
    async Task Delete()
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        Departments.Remove(selectedDepartment);
        try
        {
            await _idepartmentRepository.DeleteDepartmentAsync(selectedDepartment);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }        
    }

    [RelayCommand]
    async Task Edit()
    {
        // i'll implement this later
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }

    [RelayCommand]
    async Task Detail()
    {
        // i'll implement this later
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }
}
