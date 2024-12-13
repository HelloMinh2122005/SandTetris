using CommunityToolkit.Mvvm.ComponentModel;
using SandTetris.Entities;
using System.Collections.ObjectModel;
using SandTetris.Interfaces;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Views;

namespace SandTetris.ViewModels;

public partial class EmployeeOfTheMonthPageViewModel : ObservableObject
{
    [ObservableProperty]
    SalaryDetail bestEmployee = new();

    [ObservableProperty]
    ObservableCollection<SalaryDetail> salaryDetails = new();

    [ObservableProperty]
    string selectedMonth = "Now";

    [ObservableProperty]
    string selectedYear = "Now";

    [ObservableProperty]
    ObservableCollection<string> months = new();

    [ObservableProperty]
    ObservableCollection<string> years = new();

    readonly ISalaryDetailRepository _iSalaryRepository;
    SalaryDetail selectedSalary = new();

    public EmployeeOfTheMonthPageViewModel(ISalaryDetailRepository iSalaryRepository)
    {
        _iSalaryRepository = iSalaryRepository;
        Months.Add("Now");
        for (int i = 1; i <= 12; i++)
        {
            Months.Add(i.ToString());
        }
        Years.Add("Now");
        for (int i = 2020; i <= DateTime.Now.Year; i++)
        {
            Years.Add(i.ToString());
        }

        OnAppearing();
    }

    public async void OnAppearing()
    {
        await LoadSalaryDetails();
    }

    partial void OnSelectedMonthChanged(string value)
    {
        OnAppearing();
    }

    partial void OnSelectedYearChanged(string value)
    {
        OnAppearing();
    }

    async Task LoadSalaryDetails()
    {
        int month, year;
        if (SelectedMonth == "Now")
            month = DateTime.Now.Month;
        else
            month = int.Parse(SelectedMonth);
        if (SelectedYear == "Now")
            year = DateTime.Now.Year;
        else
            year = int.Parse(SelectedYear);

        var salaryList = await _iSalaryRepository.GetSalaryDetailsMonthYearAsync(month, year);
        SalaryDetails = new ObservableCollection<SalaryDetail>(salaryList);

        BestEmployee = SalaryDetails.FirstOrDefault() ?? new SalaryDetail();
    }

    [RelayCommand]
    void ItemSelected(SalaryDetail salary)
    {
        if (salary == null)
            return;
        selectedSalary = salary;
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedSalary == null || string.IsNullOrEmpty(selectedSalary.EmployeeId))
        {
            await Shell.Current.DisplayAlert("Error", "Please select an employee", "OK");
            return;
        }

        int month, year;
        if (SelectedMonth == "Now")
            month = DateTime.Now.Month;
        else
            month = int.Parse(SelectedMonth);
        if (SelectedYear == "Now")
            year = DateTime.Now.Year;
        else
            year = int.Parse(SelectedYear);

        await Shell.Current.GoToAsync($"{nameof(BonusSalaryPage)}", new Dictionary<string, object>
        {
            { "employeeid", selectedSalary.EmployeeId },
            { "month", month },
            { "year", year }
        });
    }

    [RelayCommand]
    async Task History()
    {
        await Shell.Current.GoToAsync($"{nameof(TopEmployeeListPage)}");
    }
}
