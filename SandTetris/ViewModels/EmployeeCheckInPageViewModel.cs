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

    private CheckIn selectedCheckIn = null;

    private readonly ICheckInRepository _checkInRepository;
    private string departmentId = "";
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();
    private readonly ISalaryService _salaryService;

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
            selectedCheckIn = checkIn;
    }

    [RelayCommand]
    private async void Appear()
    {
        if (selectedCheckIn != null && selectedCheckIn.Status != CheckInStatus.Working)
        {
            CheckInStatus preStatus = selectedCheckIn.Status;
            selectedCheckIn.Status = CheckInStatus.Working;
            var index = CheckIns.IndexOf(selectedCheckIn);
            CheckIns[index] = selectedCheckIn;

            await _checkInRepository.UpdateCheckInSummary(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year, CheckInStatus.Working, preStatus);
            await _salaryService.CalculateSalaryForEmployeeAsync(selectedCheckIn.EmployeeId, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
            selectedCheckIn = null;
        }   
    }

    [RelayCommand]
    private async void OnLeave()
    {
        if (selectedCheckIn != null && selectedCheckIn.Status != CheckInStatus.OnLeave)
        {
            CheckInStatus preStatus = selectedCheckIn.Status;
            selectedCheckIn.Status = CheckInStatus.OnLeave;
            selectedCheckInSummary.TotalOnLeave++;
            var index = CheckIns.IndexOf(selectedCheckIn);
            CheckIns[index] = selectedCheckIn;

            await _checkInRepository.UpdateCheckInSummary(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year, CheckInStatus.OnLeave, preStatus);
            await _salaryService.CalculateSalaryForEmployeeAsync(selectedCheckIn.EmployeeId, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
            selectedCheckIn = null;
        }
    }

    [RelayCommand]
    private async void Absent()
    {
        if (selectedCheckIn != null && selectedCheckIn.Status != CheckInStatus.Absent)
        {
            CheckInStatus preStatus = selectedCheckIn.Status;
            selectedCheckIn.Status = CheckInStatus.Absent;
            selectedCheckInSummary.TotalAbsent++;
            var index = CheckIns.IndexOf(selectedCheckIn);
            CheckIns[index] = selectedCheckIn;

            await _checkInRepository.UpdateCheckInSummary(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year, CheckInStatus.Absent, preStatus);
            await _salaryService.CalculateSalaryForEmployeeAsync(selectedCheckIn.EmployeeId, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
            selectedCheckIn = null;
        }
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
}
