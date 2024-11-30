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

    private readonly ICheckInRepository _checkInRepository;
    private string departmentId = "";
    private CheckInSummary selectedCheckInSummary = new CheckInSummary();

    [ObservableProperty]
    private ObservableCollection<CheckIn> selectedCheckIn = new ObservableCollection<CheckIn>();

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
            if (SelectedCheckIn.Contains(checkIn))
                SelectedCheckIn.Remove(checkIn);
            else
                SelectedCheckIn.Add(checkIn);
        }
    }

    [RelayCommand]
    private async Task Appear()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            selectedCheckInSummary.TotalWorking++;
            selectedCheckInSummary.TotalAbsent--;
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                checkin.Status = CheckInStatus.Working;
                await _checkInRepository.UpdateEmployeeCheckInAsync(checkIn.EmployeeId, checkIn.Day, checkIn.Month, checkIn.Year, CheckInStatus.Working, DateTime.Now, checkIn.Note ?? "");

                var existingCheckIn = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId && c.Day == checkIn.Day && c.Month == checkIn.Month && c.Year == checkIn.Year);
                if (existingCheckIn != null)
                {
                    await _checkInRepository.UpdateCheckInAsync(checkIn);
                    var index = CheckIns.IndexOf(existingCheckIn);
                    CheckIns[index] = checkIn;
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    [RelayCommand]
    private async Task OnLeave()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            selectedCheckInSummary.TotalOnLeave++;
            selectedCheckInSummary.TotalAbsent--;
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                checkin.Status = CheckInStatus.OnLeave;
                await _checkInRepository.UpdateEmployeeCheckInAsync(checkIn.EmployeeId, checkIn.Day, checkIn.Month, checkIn.Year, CheckInStatus.OnLeave, DateTime.Now, checkIn.Note ?? "");

                var existingCheckIn = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId && c.Day == checkIn.Day && c.Month == checkIn.Month && c.Year == checkIn.Year);
                if (existingCheckIn != null)
                {
                    await _checkInRepository.UpdateCheckInAsync(checkIn);
                    var index = CheckIns.IndexOf(existingCheckIn);
                    CheckIns[index] = checkIn;
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    [RelayCommand]
    private async Task Absent()
    {
        foreach (var checkIn in SelectedCheckIn)
        {
            selectedCheckInSummary.TotalOnLeave++;
            selectedCheckInSummary.TotalAbsent--;
            var checkin = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId);
            if (checkin != null)
            {
                checkin.Status = CheckInStatus.Absent;
                await _checkInRepository.UpdateEmployeeCheckInAsync(checkIn.EmployeeId, checkIn.Day, checkIn.Month, checkIn.Year, CheckInStatus.Absent, DateTime.Now, checkIn.Note ?? "");

                var existingCheckIn = CheckIns.FirstOrDefault(c => c.EmployeeId == checkIn.EmployeeId && c.Day == checkIn.Day && c.Month == checkIn.Month && c.Year == checkIn.Year);
                if (existingCheckIn != null)
                {
                    await _checkInRepository.UpdateCheckInAsync(checkIn);
                    var index = CheckIns.IndexOf(existingCheckIn);
                    CheckIns[index] = checkIn;
                }
            }
        }
        SelectedCheckIn.Clear();
    }

    [RelayCommand]
    async Task Search()
    {
        var checkinList = await _checkInRepository.GetCheckInsForDepartmentAsync(departmentId, selectedCheckInSummary.Day, selectedCheckInSummary.Month, selectedCheckInSummary.Year);
        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            checkinList = checkinList.Where(d =>
                d.EmployeeId.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
                || d.Employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase));
        }

        CheckIns.Clear();
        foreach (var check in checkinList)
        {
            CheckIns.Add(check);
        }
    }

    [RelayCommand]
    async Task Filter()
    {
        await Shell.Current.DisplayAlert("ok", "ok", "ok");
    }
}
