using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Data;
using SandTetris.Entities;
using SandTetris.Interfaces;
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
    private string searchbar = "";

    [ObservableProperty]
    private int numberOfEmployees = 0;

    [ObservableProperty]
    private ObservableCollection<CheckInSummary> checkInSummaries = new ObservableCollection<CheckInSummary>();

    [ObservableProperty]
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();

    private readonly ICheckInRepository _checkInRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private string departmentId = "";
    private int month = DateTime.Now.Month;
    private int year = DateTime.Now.Year;

    public CheckInDetailPageViewModel(ICheckInRepository checkInRepository, IDepartmentRepository departmentRepository)
    {
        _checkInRepository = checkInRepository;
        _departmentRepository = departmentRepository;
    }
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentId = (string)query["departmentId"];
        month = (int)query["month"];
        year = (int)query["year"];

        NumberOfEmployees = await _departmentRepository.GetTotalDepartmentEmployees(departmentId);

        await LoadCheckIn();
    }
    private async Task LoadCheckIn()
    {
        var checkInList = await _checkInRepository.GetCheckInSummariesAsync(departmentId, month, year);
        foreach (var checkIn in checkInList)
        {
            CheckInSummaries.Add(checkIn);
        }
    }

    [RelayCommand]
    async Task Search()
    {
        var tcheckinSummaries = await _checkInRepository.GetCheckInSummariesAsync(departmentId, month, year);
        if (int.TryParse(Searchbar, out int searchValue))
        {
            tcheckinSummaries = tcheckinSummaries.Where(d =>
                d.Day == searchValue || d.Month == searchValue || d.Year == searchValue);
        }

        CheckInSummaries.Clear();
        foreach (var checkInSummary in tcheckinSummaries)
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
    async Task IntemSelected(CheckInSummary checkIn)
    {
        SelectedCheckInSummary = checkIn;
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }

    [RelayCommand]
    async Task Edit()
    {
        if (SelectedCheckInSummary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a check-in", "OK");
            return;
        }
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
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
