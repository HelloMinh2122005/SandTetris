using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Views;
using SandTetris.Services;
using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.Bibliography;

namespace SandTetris.ViewModels;

public partial class SalaryPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string searchbar = "";

    [ObservableProperty]
    private ObservableCollection<SalaryDetail> salaryDetails = new ObservableCollection<SalaryDetail>();

    [ObservableProperty]
    private string selectedMonth = "Now";

    [ObservableProperty]
    private string selectedYear = "Now";

    [ObservableProperty]
    private ObservableCollection<string> months = new();

    [ObservableProperty]
    private ObservableCollection<string> years = new();

    [ObservableProperty]
    bool isVisible = false;

    [ObservableProperty]
    bool isNVisible = true;

    private SalaryDetail selectedSalary = null;
    private readonly ISalaryService _salaryService;
    private readonly ISalaryDetailRepository _salaryDetailRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private SalaryDetailSummary SalaryDetailSummaryPara = new SalaryDetailSummary();

    public SalaryPageViewModel(ISalaryService salaryService, ISalaryDetailRepository salaryDetailRepository, IDepartmentRepository departmentRepository)
    {
        _salaryService = salaryService;
        _salaryDetailRepository = salaryDetailRepository;
        _departmentRepository = departmentRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("salarySummary"))
        {
            SalaryDetailSummaryPara = (SalaryDetailSummary)query["salarySummary"];
            query.Remove("salarySummary");

            await LoadSalaryDetails();
        }
        if (query.ContainsKey("edit"))
        {
            var updatedSalary = (SalaryDetail)query["edit"];
            var existingSalary = SalaryDetails.FirstOrDefault(d => d.EmployeeId == updatedSalary.EmployeeId);
            updatedSalary.FinalSalary = await _salaryService.CalculateSalaryForEmployeeAsync(updatedSalary.EmployeeId, updatedSalary.Month, updatedSalary.Year);
            query.Remove("edit");
            if (existingSalary != null)
            {
                SalaryDetailSummaryPara.TotalSpent -= existingSalary.FinalSalary;
                SalaryDetailSummaryPara.TotalSpent += updatedSalary.FinalSalary;
                var index = SalaryDetails.IndexOf(existingSalary);
                SalaryDetails[index] = updatedSalary;
            }
        }
        if (query.ContainsKey("edit1"))
        {
            var updatedSalary = (SalaryDetail)query["edit1"];
            var existingSalary = SalaryDetails.FirstOrDefault(d => d.EmployeeId == updatedSalary.EmployeeId);
            updatedSalary.FinalSalary = await _salaryService.CalculateSalaryForEmployeeAsync(updatedSalary.EmployeeId, updatedSalary.Month, updatedSalary.Year);
            query.Remove("edit1");
            if (existingSalary != null)
            {
                var index = SalaryDetails.IndexOf(existingSalary);
                SalaryDetails[index] = updatedSalary;
            }
        }
        if (query.ContainsKey("viewall"))
        {
            IsVisible = true;
            IsNVisible = false;
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
            query.Remove("viewall");
        }
    }

    partial void OnSelectedMonthChanged(string value)
    {
        OnAppearing();
    }

    partial void OnSelectedYearChanged(string value)
    {
        OnAppearing();
    }

    public async void OnAppearing()
    {
        await LoadSalaryDetailsAll();
    }

    async Task LoadSalaryDetailsAll()
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

        var salaryLists = await _salaryDetailRepository.GetSalaryDetailsAsync(month, year);
        SalaryDetails = new ObservableCollection<SalaryDetail>(salaryLists);
    }

    async Task LoadSalaryDetails()
    {
        var salaryLists = await _salaryDetailRepository.GetSalaryDetailsForDepartmentAsync(
            SalaryDetailSummaryPara.DepartmentId, 
            SalaryDetailSummaryPara.Month, 
            SalaryDetailSummaryPara.Year);

        if (salaryLists.Count() == 0)
        {
            salaryLists = await _salaryDetailRepository.AddSalaryDetailsForDepartmentAsync(
                SalaryDetailSummaryPara.DepartmentId,
                SalaryDetailSummaryPara.Month,
                SalaryDetailSummaryPara.Year);
        }
        SalaryDetails = new ObservableCollection<SalaryDetail>(salaryLists);
    }

    [RelayCommand]
    void OnItemSelected(SalaryDetail salaryDetail)
    {
        selectedSalary = salaryDetail;
    }

    [RelayCommand]
    async Task Search()
    {
        IEnumerable<SalaryDetail> salaries;
        if (IsVisible)
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

            salaries = await _salaryDetailRepository.GetSalaryDetailsAsync(month, year);
        }
        else
            salaries = await _salaryDetailRepository.GetSalaryDetailsForDepartmentAsync(
                SalaryDetailSummaryPara.DepartmentId,
                SalaryDetailSummaryPara.Month,
                SalaryDetailSummaryPara.Year);

        if (!string.IsNullOrWhiteSpace(Searchbar))
        {
            salaries = salaries.Where(sa =>
                sa.EmployeeId.Contains(Searchbar, StringComparison.OrdinalIgnoreCase)
                || sa.Employee.FullName.Contains(Searchbar, StringComparison.OrdinalIgnoreCase));
        }

        SalaryDetails.Clear();
        foreach (var salary in salaries)
        {
            SalaryDetails.Add(salary);
        }
    }

    [RelayCommand] 
    async Task Edit()
    {
        if (selectedSalary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a salary", "OK");
            return;
        }

        string command;
        if (IsVisible)
            command = "edit1";
        else
            command = "edit";

        await Shell.Current.GoToAsync($"{nameof(SalaryDetailPage)}", new Dictionary<string, object>
        {
            { "employeeId", selectedSalary.EmployeeId },
            { "month", selectedSalary.Month },
            { "year", selectedSalary.Year },
            { "command", command }
        });
    }

    [RelayCommand]
    async Task Delete()
    {
        // TODO: yank this shit
        if (selectedSalary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a salary", "OK");
            return;
        }

        try
        {
            await _salaryDetailRepository.DeleteSalaryDetailAsync(selectedSalary.EmployeeId, selectedSalary.Month, selectedSalary.Year);
            SalaryDetails.Remove(selectedSalary);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task Detail()
    {
        if (selectedSalary == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a salary", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(SalaryDetailPage)}", new Dictionary<string, object>
        {
            { "employeeId", selectedSalary.EmployeeId },
            { "month", selectedSalary.Month },
            { "year", selectedSalary.Year },
            { "command", "detail" }
        });
    }

    [RelayCommand]
    async Task Export()
    {
        ExportFilePDF export = new ExportFilePDF(_salaryDetailRepository, _departmentRepository);
        await export.ExportPDF(SalaryDetailSummaryPara.DepartmentId, SalaryDetailSummaryPara.Month, SalaryDetailSummaryPara.Year);
    }
}
