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

    private readonly ICheckInRepository _checkInRepository;
    private string departmentId = "";
    private List<CheckIn> selectedCheckIn = [];
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();

    public EmployeeCheckInPageViewModel(ICheckInRepository checkInRepository)
    {
        _checkInRepository = checkInRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        departmentId = (string)query["departmentId"];
        selectedCheckInSummary = (CheckInSummary)query["CheckInSummary"];

        await LoadCheckIns(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
    }

    private async Task LoadCheckIns(string departmentID, int Day, int Month, int Year)
    {
        var checkInList = await _checkInRepository.GetCheckInsForDepartmentAsync(departmentID, Day, Month, Year);
        CheckIns = new ObservableCollection<CheckIn>(checkInList);
    }

    [RelayCommand]
    private void ItemSelected(CheckIn checkIn)
    {
        if (checkIn != null)
        {
            selectedCheckIn.Add(checkIn);
        }
    }

    [RelayCommand]
    private async Task Appear()
    {
        foreach (var checkIn in selectedCheckIn)
        {
            selectedCheckInSummary.TotalWorking++;
            selectedCheckInSummary.TotalAbsent--;
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                checkin.Status = CheckInStatus.Working;
                await _checkInRepository.UpdateEmployeeCheckInAsync(checkIn.EmployeeId, checkIn.Day, checkIn.Month, checkIn.Year, CheckInStatus.Working, DateTime.Now);
            }
        }
        selectedCheckIn.Clear();
    }

    [RelayCommand]
    private async Task OnLeave()
    {
        foreach (var checkIn in selectedCheckIn)
        {
            selectedCheckInSummary.TotalOnLeave++;
            selectedCheckInSummary.TotalAbsent--;
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                checkin.Status = CheckInStatus.Working;
                await _checkInRepository.UpdateEmployeeCheckInAsync(checkIn.EmployeeId, checkIn.Day, checkIn.Month, checkIn.Year, CheckInStatus.OnLeave, DateTime.Now);
            }
        }
        selectedCheckIn.Clear();
    }
}
