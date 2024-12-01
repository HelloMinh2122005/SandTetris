using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class SalaryDetailPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private SalaryDetail salary = new SalaryDetail();

    [ObservableProperty]
    private int finalSalary = 0;

    [ObservableProperty]
    private bool isReadOnly = true;

    [ObservableProperty]
    private bool isVisible = false;

    private string employeeID = ""; 
    private int month = 0;
    private int year = 0;

    private readonly ISalaryDetailRepository _salaryDetailRepository;
    private readonly ISalaryService _salaryService;

    public SalaryDetailPageViewModel(ISalaryDetailRepository salaryDetailRepository, ISalaryService salaryService)
    {
        _salaryDetailRepository = salaryDetailRepository;
        _salaryService = salaryService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("employeeId"))
        {
            employeeID = query["employeeId"].ToString() ?? "";
            month = int.Parse(query["month"].ToString() ?? "0");
            year = int.Parse(query["year"].ToString() ?? "0");

            query.Remove("employeeId");

            string command = query["command"].ToString() ?? "";
            if (command == "edit")
            {
                IsVisible = true;
                IsReadOnly = false;
            }

            try
            {
                Salary = await _salaryDetailRepository.GetSalaryDetailAsync(employeeID, month, year);
                FinalSalary = Salary.FinalSalary;
            }
            catch
            {
                await Shell.Current.DisplayAlert(employeeID, "No salary detail found", "OK");
            }
        }
    }

    [RelayCommand]
    async Task Save()
    {
        if (Salary.BaseSalary == 0)
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a base salary", "OK");
            return;
        }
        Salary.FinalSalary = await _salaryService.CalculateSalaryForEmployeeAsync(Salary.EmployeeId, Salary.Month, Salary.Year);
        FinalSalary = Salary.FinalSalary;
        await Shell.Current.DisplayAlert("Success", "Salary detail saved", "OK");
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
