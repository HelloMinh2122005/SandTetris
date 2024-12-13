using CommunityToolkit.Mvvm.ComponentModel;
using SandTetris.Interfaces;
using SandTetris.Entities;
using CommunityToolkit.Mvvm.Input;

namespace SandTetris.ViewModels;

public partial class BonusSalaryPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    int finalSalary = 0;

    [ObservableProperty]
    int deposit = 0;

    [ObservableProperty]
    SalaryDetail thisSalaryDetail = new();

    readonly ISalaryDetailRepository _iSalaryRepository;
    public BonusSalaryPageViewModel(ISalaryDetailRepository iSalaryRepository)
    {
        _iSalaryRepository = iSalaryRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string employeeID = (string)query["employeeId"];
        int month = (int)query["month"];
        int year = (int)query["year"];

        ThisSalaryDetail = await _iSalaryRepository.GetSalaryDetailAsync(employeeID, month, year);
        FinalSalary = ThisSalaryDetail.FinalSalary;
    }

    [RelayCommand]
    async Task AddDeposit()
    {
        string deposit = await Shell.Current.DisplayPromptAsync("Deposit", "Enter the deposit amount", "OK", "Cancel", "0", 10000, Keyboard.Numeric);
        if (deposit == null)
            return;
        int depositAmount = int.Parse(deposit);
        FinalSalary += depositAmount;
        Deposit += depositAmount;
        await _iSalaryRepository.AddDepositAsync(ThisSalaryDetail.EmployeeId, ThisSalaryDetail.Month, ThisSalaryDetail.Year, depositAmount);
    }

    [RelayCommand]
    async Task RemoveDeposit()
    {
        bool ac = await Shell.Current.DisplayAlert("Deposit", "Are you sure want to remove deposit", "Yes", "No");
        if (!ac)
            return;
        FinalSalary -= Deposit;
        Deposit = 0;
        await _iSalaryRepository.RemoveDepositAsync(ThisSalaryDetail.EmployeeId, ThisSalaryDetail.Month, ThisSalaryDetail.Year);
    }
}
