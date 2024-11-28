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

public partial class DepartmentCheckInPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    ObservableCollection<Department> departments = new ObservableCollection<Department>();

    [ObservableProperty]
    string searchbar = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

    }

    public DepartmentCheckInPageViewModel(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
        OnAppearing();
    }

    private async void OnAppearing()
    {
        await LoadDepartments();
    }

    private async Task LoadDepartments()
    {
        var departmentList = await _departmentRepository.GetDepartmentsAsync();
        Departments = new ObservableCollection<Department>(departmentList);
    }

    private readonly IDepartmentRepository _departmentRepository;
    private Department selectedDepartment = new Department { Name = "" };

    [RelayCommand]
    public void ItemSelected(Department department)
    {
        selectedDepartment = department;
    }

    [RelayCommand]
    async Task Search()
    {
        var departmentList = await _departmentRepository.GetDepartmentsAsync();
        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            departmentList = departmentList.Where(d => d.Name.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        Departments.Clear();
        foreach (var department in departmentList)
        {
            Departments.Add(department);
        }
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(CheckInDetailPage)}", new Dictionary<string, object>
        {
            { "departmentID", selectedDepartment.Id }
        });
    }

    [RelayCommand]
    async Task Filter()
    {
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }
}