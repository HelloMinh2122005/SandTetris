using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Data;
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

public partial class CheckInDetailPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string searchBar = "";

    [ObservableProperty]
    private int numberOfEmployees = 0;

    [ObservableProperty]
    private ObservableCollection<CheckInSummary> checkInSummaries = new();

    [ObservableProperty]
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();

    [ObservableProperty]
    private string selectedMonth = "Now";

    [ObservableProperty]
    private string selectedYear = "Now";

    partial void OnSelectedMonthChanged(string value)
    {
        OnAppearing();
    }

    partial void OnSelectedYearChanged(string value)
    {
        OnAppearing();
    }

    [ObservableProperty]
    private ObservableCollection<string> months = new();

    [ObservableProperty]
    private ObservableCollection<string> years = new();

    private readonly ICheckInRepository _checkInRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private string departmentId = "";

    public CheckInDetailPageViewModel(ICheckInRepository checkInRepository, IDepartmentRepository departmentRepository)
    {
        _checkInRepository = checkInRepository;
        _departmentRepository = departmentRepository;

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
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentId = (string)query["departmentId"];
        NumberOfEmployees = await _departmentRepository.GetTotalDepartmentEmployees(departmentId);
        OnAppearing();
    }

    private async void OnAppearing()
    {
        await LoadCheckInSummaries();
    }

    private async Task LoadCheckInSummaries()
    {
        if (SelectedMonth == "Now" || SelectedYear == "Now")
        {
            var checkInList = await _checkInRepository.GetAllCheckInSummariesAsync(departmentId);
            CheckInSummaries = new ObservableCollection<CheckInSummary>(checkInList);
        }
        else
        {
            var checkInList = await _checkInRepository.GetCheckInSummariesAsync(departmentId, int.Parse(SelectedMonth), int.Parse(SelectedYear));
            CheckInSummaries = new ObservableCollection<CheckInSummary>(checkInList);
        }
    }

    [RelayCommand]
    async Task Search()
    {
        IEnumerable<CheckInSummary> checkinSummaries;

        if (SelectedMonth == "Now" || SelectedYear == "Now")
        {
            checkinSummaries = await _checkInRepository.GetAllCheckInSummariesAsync(departmentId);
        }
        else
        {
            checkinSummaries = await _checkInRepository.GetCheckInSummariesAsync(departmentId, int.Parse(SelectedMonth), int.Parse(SelectedYear));
        }

        if (int.TryParse(SearchBar, out int searchValue))
        {
            checkinSummaries = checkinSummaries.Where(d =>
                d.Day == searchValue || d.Month == searchValue || d.Year == searchValue);
        }

        CheckInSummaries.Clear();
        foreach (var checkInSummary in checkinSummaries)
        {
            CheckInSummaries.Add(checkInSummary);
        }
    }

    [RelayCommand]
    async Task Filter()
    {
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }

    [RelayCommand]
    public void ItemSelected(CheckInSummary checkIn)
    {
        SelectedCheckInSummary = checkIn;
    }

    [RelayCommand]
    async Task Add()
    {
        try
        {
            await _checkInRepository.AddCheckInsForDepartmentAsync(departmentId, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            return;
        }
        CheckInSummaries.Add(new CheckInSummary
        {
            Day = DateTime.Now.Day,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year,
            TotalWorking = 0,
            TotalOnLeave = 0,
            TotalAbsent = NumberOfEmployees
        });
    }

    [RelayCommand]
    async Task Edit()
    {
        if (SelectedCheckInSummary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a check-in", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(EmployeeCheckInPage)}", new Dictionary<string, object>
        {
            { "departmentId", departmentId },
            { "CheckInSummary", SelectedCheckInSummary }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        if (SelectedCheckInSummary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a check-in", "OK");
            return;
        }
        await _checkInRepository.DeleteCheckInForDepartmentAsync(departmentId, SelectedCheckInSummary.Day, SelectedCheckInSummary.Month, SelectedCheckInSummary.Year);
        CheckInSummaries.Remove(SelectedCheckInSummary);
    }
}
