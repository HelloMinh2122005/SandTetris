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

namespace SandTetris.ViewModels.DepartmentViewModel;
// this page will be used to display the list of departments
public partial class DepartmentListViewModel : ObservableObject, IQueryAttributable
{
    public DepartmentListViewModel(IDepartmentRepository iDepartmentRepo)
    {
        _iDepartmentRepo = iDepartmentRepo;
        Departments = new ObservableCollection<Department>();
        Searchbar = "";
        LoadDepartments();
    }

    private readonly IDepartmentRepository _iDepartmentRepo;

    [ObservableProperty]
    // this is where the list of departments should be binded to  
    private ObservableCollection<Department> departments;

    [ObservableProperty]
    // this is where the search bar should be binded to   
    private string searchbar;

    private List<Department> SelectedDepartments;

    private async void LoadDepartments()
    {
        var departments = await _iDepartmentRepo.GetDepartmentsAsync();
        foreach (var department in departments)
        {
            Departments.Add(department);
        }
    }

    async void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("Add"))
        {
            var newDepartment = (Department)query["Add"];
            Departments.Add(newDepartment);
            await _iDepartmentRepo.AddDepartmentAsync(newDepartment);
        }
        else if (query.ContainsKey("Update"))
        {
            var updatedDepartment = (Department)query["Update"];
            var index = Departments.IndexOf(updatedDepartment);
            Departments[index] = updatedDepartment;
            await _iDepartmentRepo.UpdateDepartmentAsync(updatedDepartment);
        }
    } 

    [RelayCommand]
    // bind the IsChecked of check box to isSelected, and the Command to SelectDepartment
    public void SelectDepartment()
    {
        // i'll come back to this later
    }

    [RelayCommand]
    // this is where the Add button should be binded to  
    async Task Add()
    {
        await Shell.Current.GoToAsync($"DepartmentInfoPage", new Dictionary<string, object>
        {
            {"Command", "Add" }
        });
    }

    [RelayCommand]
    // this is where the Delete button should be binded to  
    async Task Delete()
    {
        if (SelectedDepartments.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No department selected", "OK");
            return;
        }
        foreach (var department in SelectedDepartments.ToList())
        {
            await _iDepartmentRepo.DeleteDepartmentAsync(department);
            Departments.Remove(department);
        }
        SelectedDepartments.Clear();
    }

    [RelayCommand]
    // this is where the Edit button should be binded to  
    async Task Edit()
    {
        if (SelectedDepartments.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No department selected", "OK");
            return;
        }
        if (SelectedDepartments.Count > 1)
        {
            await Shell.Current.DisplayAlert("Error", "Multiple departments selected", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"DepartmentInfoPage", new Dictionary<string, object>
        {
            {"DepartmentPara", SelectedDepartments.First() },
            {"Command", "Edit" }
        });
    }

    [RelayCommand]
    // this is where the Detail button should be binded to
    async Task Detail()
    {
        if (SelectedDepartments.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No department selected", "OK");
            return;
        }
        if (SelectedDepartments.Count > 1)
        {
            await Shell.Current.DisplayAlert("Error", "Multiple departments selected", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"EmployeeListPage", new Dictionary<string, object>
        {
            {"DepartmentPara", SelectedDepartments.First().Id }
        });
    }

    [RelayCommand]
    // this is where the Search button should be binded to  
    async Task Search()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    // this is where the Filter button should be binded to  
    async Task Filter()
    {
        await Task.CompletedTask;
    }
}