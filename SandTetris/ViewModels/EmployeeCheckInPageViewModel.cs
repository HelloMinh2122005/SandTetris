using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Data;
using SandTetris.Entities;
using SandTetris.Entities.Dto;
using SandTetris.Interfaces;
using SandTetris.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class EmployeeCheckInPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<CheckInDto> checkIns = [];

    [ObservableProperty]
    private string searchbar = "";

    [ObservableProperty]
    private ObservableCollection<object> selectedItems = [];

    private readonly ICheckInRepository _checkInRepository;
    private string departmentId = "";
    private CheckInSummary summary = new();
    private readonly ISalaryService _salaryService;
    private readonly DatabaseService _databaseService;

    public EmployeeCheckInPageViewModel(ICheckInRepository checkInRepository, ISalaryService salaryService, DatabaseService databaseService)
    {
        _checkInRepository = checkInRepository;
        _salaryService = salaryService;
        _databaseService = databaseService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentId = (string)query["departmentId"];
        summary = (CheckInSummary)query["CheckInSummary"];

        await LoadCheckIns(departmentId, summary.Day, summary.Month, summary.Year);
    }

    private async Task LoadCheckIns(string departmentID, int day, int month, int year)
    {
        var checkInList = await _checkInRepository.GetCheckInsForDepartmentAsync(departmentID, day, month, year);
        CheckIns = new(checkInList.Select(x => CheckInDto.FromEntity(x)));
    }

    private async Task OnUpdateCheckIn(CheckInStatus targetStatus)
    {
        if (SelectedItems.Count == 0)
        {
            await Shell.Current.DisplayAlert("No employee selected", "Please select an employee first", "OK");
            return;
        }

        for (int i = 0; i < SelectedItems.Count; i++)
        {
            var selectedItem = (CheckInDto)SelectedItems[i];

            if (selectedItem.Status == targetStatus) return;

            // This is dumb. So dumb. Yank it. Please. Why tf are we calculating salary here? Query dipshit.
            if (selectedItem.Employee is null)
            {
                throw new InvalidDataException("Employee in CheckInDto shouldnt be null");
            }

            var checkIn = await _checkInRepository.GetCheckInByIdAsync(selectedItem.Employee.Id, summary.Day, summary.Month, summary.Year) 
                ?? throw new InvalidDataException("CheckIn should not be null");

            var index = CheckIns.IndexOf(selectedItem); 
            var checkInToEdit = CheckIns[index];
            checkInToEdit.Status = targetStatus;
            CheckIns[index] = checkInToEdit;
            SelectedItems.Insert(i, CheckIns[index]);
            // actually update the db
            // TODO: move this to the repository
            checkIn.Status = targetStatus;
            checkIn.CheckInTime = DateTime.Now;

            // i still not figure out how to not calculate the salary here but still trigger other pages to update the final salary
            // especially when the salary is calculated for the new month or the new employee
            await _salaryService.CalculateSalaryForEmployeeAsync(
                checkIn.EmployeeId,
                summary.Month,
                summary.Year
            );
        }
        await _databaseService.DataContext.SaveChangesAsync();
    }

    [RelayCommand]
    private void SelectAll()
    {
        SelectedItems = new(CheckIns);
    }

    [RelayCommand]
    private void DeselectAll()
    {
        SelectedItems.Clear();
    }

    [RelayCommand]
    private async void Appear()
    {
        await OnUpdateCheckIn(CheckInStatus.Working);
    }

    [RelayCommand]
    private async void OnLeave()
    {
        await OnUpdateCheckIn(CheckInStatus.OnLeave);
    }

    [RelayCommand]
    private async void Absent()
    {
        await OnUpdateCheckIn(CheckInStatus.Absent);
    }

    [RelayCommand]
    private async Task Search()
    {
        var checkinList = await _checkInRepository.GetCheckInsForDepartmentAsync(
            departmentId,
            summary.Day,
            summary.Month,
            summary.Year
        );

        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            checkinList = checkinList.Where(d =>
                d.EmployeeId.Contains(Searchbar, StringComparison.OrdinalIgnoreCase) ||
                d.Employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
            );
        }
        CheckIns = new(checkinList.Select(x => CheckInDto.FromEntity(x)));
    }
}
