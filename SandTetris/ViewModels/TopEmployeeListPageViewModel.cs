using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using System.Collections.ObjectModel;
using SandTetris.Interfaces;
using SandTetris.Views;

namespace SandTetris.ViewModels;

public partial class TopEmployeeListPageViewModel : ObservableObject
{
    [ObservableProperty]
    string searchbar = "";

    [ObservableProperty]
    ObservableCollection<SalaryDetail> salaryDetails = new();

    ISalaryDetailRepository _salaryDetailRepository;
    SalaryDetail selectedSalary = new();

    public TopEmployeeListPageViewModel(ISalaryDetailRepository salaryDetailRepository)
    {
        _salaryDetailRepository = salaryDetailRepository;
    }

    public async void OnAppearing()
    {
        await Search();
    }

    [RelayCommand]
    async Task Search()
    {
        IEnumerable<SalaryDetail> salaries = await _salaryDetailRepository.GetTopEmployeeAsync();

        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            salaries = salaries.Where(sa =>
                sa.EmployeeId.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
                || sa.Employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase));
        }

        SalaryDetails.Clear();
        foreach (var salary in salaries)
        {
            SalaryDetails.Add(salary);
        }
    }

    [RelayCommand]
    void ItemSelected(SalaryDetail salaryDetail)
    {
        if (salaryDetail == null)
            return;
        selectedSalary = salaryDetail;
    }

    [RelayCommand]
    async Task Edit()
    {
        if (selectedSalary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a salary", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(BonusSalaryPage)}", new Dictionary<string, object>
        {
            { "employeeId", selectedSalary.EmployeeId },
            { "month", selectedSalary.Month },
            { "year", selectedSalary.Year }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        if (selectedSalary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a salary", "OK");
            return;
        }

        bool ac = await Shell.Current.DisplayAlert("Warning", "Are you sure you want to delete this deposit?", "Yes", "No");
        if (!ac)
            return;

        await _salaryDetailRepository.RemoveDepositAsync(selectedSalary.EmployeeId, selectedSalary.Month, selectedSalary.Year);
        
        SalaryDetails.Remove(selectedSalary);
    }
}
