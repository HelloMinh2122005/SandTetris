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

public partial class DepartmentPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchbar = "";

    [ObservableProperty]    
    private ObservableCollection<Department> departments = new ObservableCollection<Department>();

    public DepartmentPageViewModel(IDepartmentRepository departmentRepository)
    {
        _idepartmentRepository = departmentRepository;
    }

    private readonly IDepartmentRepository _idepartmentRepository;

    [RelayCommand]
    async Task Filter()
    {
        // i'll implement this later
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }

    [RelayCommand]
    async Task Add()
    {
        // i'll implement this later
        await Shell.Current.DisplayAlert("ok","ok","ok");
    }

    [RelayCommand]
    async Task Delete(Department selectedDepartment)
    {
        if (selectedDepartment == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a department", "OK");
            return;
        }
        Departments.Remove(selectedDepartment);
        await _idepartmentRepository.DeleteDepartmentAsync(selectedDepartment);
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
