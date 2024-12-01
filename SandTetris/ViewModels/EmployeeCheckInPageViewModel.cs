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

public partial class EmployeeCheckInPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<CheckIn> checkIns = new ObservableCollection<CheckIn>();

    [ObservableProperty]
    private string searchbar = "";

    [ObservableProperty]
    private ObservableCollection<CheckIn> selectedCheckIn = new ObservableCollection<CheckIn>();

    private readonly ICheckInRepository _checkInRepository;
    private string departmentId = "";
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();
    private readonly ISalaryService _salaryService;

    private ObservableCollection<CheckIn> modifiedCheckIns = new ObservableCollection<CheckIn>();

    private int totalWorkingChanges = 0;
    private int totalOnLeaveChanges = 0;
    private int totalAbsentChanges = 0;

    public EmployeeCheckInPageViewModel(ICheckInRepository checkInRepository, ISalaryService salaryService)
    {
        _checkInRepository = checkInRepository;
        _salaryService = salaryService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentId = (string)query["departmentId"];
        selectedCheckInSummary = (CheckInSummary)query["CheckInSummary"];

        await LoadCheckIns(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
    }

    private async Task LoadCheckIns(string departmentID, int day, int month, int year)
    {
        var checkInList = await _checkInRepository.GetCheckInsForDepartmentAsync(departmentID, day, month, year);
        CheckIns = new ObservableCollection<CheckIn>(checkInList);
    }

    [RelayCommand]
    private void ItemSelected(CheckIn checkIn)
    {
        if (checkIn != null)
        {
            if (SelectedCheckIn.Contains(checkIn))
                SelectedCheckIn.Remove(checkIn);
            else
                SelectedCheckIn.Add(checkIn);
        }
    }

    [RelayCommand]
    private void Appear()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                var previousStatus = checkin.Status;
                checkin.Status = CheckInStatus.Working;

                var index = CheckIns.IndexOf(checkin);
                CheckIns[index] = checkin;

                AdjustSummaryCounters(previousStatus, CheckInStatus.Working);

                if (!modifiedCheckIns.Contains(checkin))
                {
                    modifiedCheckIns.Add(checkin);
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    [RelayCommand]
    private void OnLeave()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                var previousStatus = checkin.Status;
                checkin.Status = CheckInStatus.OnLeave;

                var index = CheckIns.IndexOf(checkin);
                CheckIns[index] = checkin;

                AdjustSummaryCounters(previousStatus, CheckInStatus.OnLeave);

                if (!modifiedCheckIns.Contains(checkin))
                {
                    modifiedCheckIns.Add(checkin);
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    [RelayCommand]
    private void Absent()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                var previousStatus = checkin.Status;
                checkin.Status = CheckInStatus.Absent;

                var index = CheckIns.IndexOf(checkin);
                CheckIns[index] = checkin;

                AdjustSummaryCounters(previousStatus, CheckInStatus.Absent);

                if (!modifiedCheckIns.Contains(checkin))
                {
                    modifiedCheckIns.Add(checkin);
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    private void AdjustSummaryCounters(CheckInStatus previousStatus, CheckInStatus newStatus)
    {
        if (previousStatus != newStatus)
        {
            if (previousStatus == CheckInStatus.Working)
                totalWorkingChanges--;
            else if (previousStatus == CheckInStatus.OnLeave)
                totalOnLeaveChanges--;
            else if (previousStatus == CheckInStatus.Absent)
                totalAbsentChanges--;

            if (newStatus == CheckInStatus.Working)
                totalWorkingChanges++;
            else if (newStatus == CheckInStatus.OnLeave)
                totalOnLeaveChanges++;
            else if (newStatus == CheckInStatus.Absent)
                totalAbsentChanges++;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        foreach (var checkIn in modifiedCheckIns)
        {
            await _checkInRepository.UpdateEmployeeCheckInAsync(
                checkIn.EmployeeId,
                checkIn.Day,
                checkIn.Month,
                checkIn.Year,
                checkIn.Status,
                DateTime.Now,
                checkIn.Note ?? string.Empty
            );


            // update the salary after updating the checkin
            await _salaryService.CalculateSalaryForEmployeeAsync(
                checkIn.EmployeeId,
                checkIn.Month,
                checkIn.Year
            );
        }

        selectedCheckInSummary.TotalWorking += totalWorkingChanges;
        selectedCheckInSummary.TotalOnLeave += totalOnLeaveChanges;
        selectedCheckInSummary.TotalAbsent += totalAbsentChanges;

        modifiedCheckIns.Clear();
        totalWorkingChanges = 0;
        totalOnLeaveChanges = 0;
        totalAbsentChanges = 0;

        await Shell.Current.DisplayAlert("Success", "Saved successfully", "OK");
    }

    [RelayCommand]
    private async Task Search()
    {
        var checkinList = await _checkInRepository.GetCheckInsForDepartmentAsync(
            departmentId,
            selectedCheckInSummary.Day,
            selectedCheckInSummary.Month,
            selectedCheckInSummary.Year
        );

        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            checkinList = checkinList.Where(d =>
                d.EmployeeId.Contains(Searchbar, StringComparison.OrdinalIgnoreCase) ||
                d.Employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
            );
        }

        CheckIns = new ObservableCollection<CheckIn>(checkinList);
    }

    [RelayCommand]
    private async Task Filter()
    {
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }
}
