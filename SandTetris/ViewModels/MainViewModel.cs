using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SandTetris.Entities;
using SandTetris.Services;

namespace SandTetris.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public static readonly FilePickerFileType DbFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { "application/x-sqlite3", ".db" } },
            { DevicePlatform.macOS, new[] { "application/x-sqlite3", ".db" } },
            { DevicePlatform.Android, new[] { "application/x-sqlite3", ".db" } },
            { DevicePlatform.iOS, new[] { "application/x-sqlite3", ".db" } },
            // Add other platforms if necessary
        });

    public static readonly FilePickerFileType ExcelFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
        { DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } },
        { DevicePlatform.macOS, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel", ".xlsx", ".xls" } },
        { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel", ".xlsx", ".xls" } },
        { DevicePlatform.iOS, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel", ".xlsx", ".xls" } },
            // Add other platforms if necessary
        });

    [ObservableProperty]
    private string dataFilePath = "";

    [ObservableProperty]
    private bool showLoadingScreen = false;

    private readonly DatabaseService dbService;

    public MainViewModel(DatabaseService databaseService)
    {
        dbService = databaseService;
        OnAppearing();
    }

    public async void OnAppearing()
    {
        await UseDefaultDatabase();
    }

    public async Task InitializeDbAndNavigate()
    {
        var dbPath = Preferences.Get("DBPATH", "");
        if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
        {
            // Prompt user to select a database or use default
            return;
        }

        ShowLoadingScreen = true;

        await TryInitializeDatabaseAsync(dbPath);
    }

    private async Task TryInitializeDatabaseAsync(string dbPath)
    {
        try
        {
            await dbService.Initialize(dbPath);
            ShowLoadingScreen = false;
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task UseDefaultDatabase()
    {
        var defaultPath = Path.Combine(FileSystem.AppDataDirectory, "default.db");
        await TryInitializeDatabaseAsync(defaultPath);
        Preferences.Set("DBPATH", defaultPath);
    }

    [RelayCommand]
    public async Task UseDefaultDbAsync()
    {
        try
        {
            ShowLoadingScreen = true;

            await UseDefaultDatabase();

            ShowLoadingScreen = false;

            await Shell.Current.DisplayAlert("Success", "Switched to the default local database.", "OK");
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to switch to default database: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task ImportDbAsync()
    {
        try
        {
            // Open file picker for .db files
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select Database File",
                FileTypes = DbFileType
            });

            if (result != null)
            {
                var selectedFilePath = result.FullPath;

                if (File.Exists(selectedFilePath))
                {
                    ShowLoadingScreen = true;

                    // Initialize the database with the selected file
                    await dbService.Initialize(selectedFilePath);

                    // Update the preference
                    Preferences.Set("DBPATH", selectedFilePath);

                    ShowLoadingScreen = false;

                    await Shell.Current.DisplayAlert("Success", "Database imported successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Selected file does not exist.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to import database: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task ImportExcelAsync()
    {
        try
        {
            // Open file picker for Excel files
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select Excel File",
                FileTypes = ExcelFileType
            });

            if (result != null)
            {
                var excelFilePath = result.FullPath;

                if (File.Exists(excelFilePath))
                {
                    ShowLoadingScreen = true;

                    // Read and process the Excel file
                    using (var workbook = new XLWorkbook(excelFilePath))
                    {
                        // Import Employees
                        await ImportEmployeesAsync(workbook);

                        // Import Departments
                        await ImportDepartmentsAsync(workbook);

                        // Import CheckIns
                        await ImportCheckInsAsync(workbook);

                        // Import SalaryDetails
                        await ImportSalaryDetailsAsync(workbook);
                    }

                    ShowLoadingScreen = false;

                    await Shell.Current.DisplayAlert("Success", "Excel data imported successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Selected Excel file does not exist.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to import Excel data: {ex.Message}", "OK");
        }
    }

    private async Task ImportEmployeesAsync(XLWorkbook workbook)
    {
        if (workbook.Worksheets.Contains("Employees"))
        {
            var worksheet = workbook.Worksheet("Employees");
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var employee = new Employee
                {
                    Id = row.Cell(1).GetString(),
                    FullName = row.Cell(2).GetString(),
                    DoB = row.Cell(3).GetDateTime(),
                    Title = row.Cell(4).GetString(),
                    DepartmentId = row.Cell(5).GetString(),
                    // Add other properties as needed
                };

                // Add or update employee in the database
                await dbService.AddOrUpdateEmployeeAsync(employee);
            }
        }
    }

    private async Task ImportDepartmentsAsync(XLWorkbook workbook)
    {
        if (workbook.Worksheets.Contains("Departments"))
        {
            var worksheet = workbook.Worksheet("Departments");
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var department = new Department
                {
                    Id = row.Cell(1).GetString(),
                    Name = row.Cell(2).GetString(),
                    Description = row.Cell(3).GetString(),
                    HeadOfDepartmentId = row.Cell(4).GetString()
                    // Add other properties as needed
                };

                // Add or update department in the database
                await dbService.AddOrUpdateDepartmentAsync(department);
            }
        }
    }

    private async Task ImportCheckInsAsync(XLWorkbook workbook)
    {
        if (workbook.Worksheets.Contains("CheckIns"))
        {
            var worksheet = workbook.Worksheet("CheckIns");
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var checkIn = new CheckIn
                {
                    EmployeeId = row.Cell(1).GetString(),
                    Day = row.Cell(2).GetValue<int>(),
                    Month = row.Cell(3).GetValue<int>(),
                    Year = row.Cell(4).GetValue<int>(),
                    CheckInTime = row.Cell(5).GetDateTime(),
                    Status = Enum.Parse<CheckInStatus>(row.Cell(6).GetString()),
                    Note = row.Cell(7).GetString()
                };

                // Add or update CheckIn in the database
                await dbService.AddOrUpdateCheckInAsync(checkIn);
            }
        }
    }

    private async Task ImportSalaryDetailsAsync(XLWorkbook workbook)
    {
        if (workbook.Worksheets.Contains("SalaryDetails"))
        {
            var worksheet = workbook.Worksheet("SalaryDetails");
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var salaryDetail = new SalaryDetail
                {
                    EmployeeId = row.Cell(1).GetString(),
                    Month = row.Cell(2).GetValue<int>(),
                    Year = row.Cell(3).GetValue<int>(),
                    BaseSalary = row.Cell(4).GetValue<int>(),
                    DaysAbsent = row.Cell(5).GetValue<int>(),
                    DaysOnLeave = row.Cell(6).GetValue<int>(),
                    FinalSalary = row.Cell(7).GetValue<int>()
                };

                // Add or update SalaryDetail in the database
                await dbService.AddOrUpdateSalaryDetailAsync(salaryDetail);
            }
        }
    }

    [RelayCommand]
    public async Task ExportExcelAsync()
    {
        try
        {
            // Define the path to save the Excel file
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = "SandTetris_Export.xlsx";
            string filePath = Path.Combine(documentsPath, fileName);

            ShowLoadingScreen = true;

            using (var workbook = new XLWorkbook())
            {
                // Export Employees
                var employees = await dbService.GetAllEmployeesAsync();
                var empSheet = workbook.Worksheets.Add("Employees");
                empSheet.Cell(1, 1).Value = "Id";
                empSheet.Cell(1, 2).Value = "FullName";
                empSheet.Cell(1, 3).Value = "DoB";
                empSheet.Cell(1, 4).Value = "Title";
                empSheet.Cell(1, 5).Value = "DepartmentId";
                // Add other headers as needed

                int empRow = 2;
                foreach (var emp in employees)
                {
                    empSheet.Cell(empRow, 1).Value = emp.Id;
                    empSheet.Cell(empRow, 2).Value = emp.FullName;
                    empSheet.Cell(empRow, 3).Value = emp.DoB;
                    empSheet.Cell(empRow, 4).Value = emp.Title;
                    empSheet.Cell(empRow, 5).Value = emp.DepartmentId;
                    // Add other properties as needed
                    empRow++;
                }

                // Export Departments
                var departments = await dbService.GetAllDepartmentsAsync();
                var deptSheet = workbook.Worksheets.Add("Departments");
                deptSheet.Cell(1, 1).Value = "Id";
                deptSheet.Cell(1, 2).Value = "Name";
                deptSheet.Cell(1, 3).Value = "Description";
                deptSheet.Cell(1, 4).Value = "HeadOfDepartmentId";
                // Add other headers as needed

                int deptRow = 2;
                foreach (var dept in departments)
                {
                    deptSheet.Cell(deptRow, 1).Value = dept.Id;
                    deptSheet.Cell(deptRow, 2).Value = dept.Name;
                    deptSheet.Cell(deptRow, 3).Value = dept.Description;
                    deptSheet.Cell(deptRow, 4).Value = dept.HeadOfDepartmentId;
                    // Add other properties as needed
                    deptRow++;
                }

                // Export CheckIns
                var checkIns = await dbService.GetAllCheckInsAsync();
                var checkInSheet = workbook.Worksheets.Add("CheckIns");
                checkInSheet.Cell(1, 1).Value = "EmployeeId";
                checkInSheet.Cell(1, 2).Value = "Day";
                checkInSheet.Cell(1, 3).Value = "Month";
                checkInSheet.Cell(1, 4).Value = "Year";
                checkInSheet.Cell(1, 5).Value = "CheckInTime";
                checkInSheet.Cell(1, 6).Value = "Status";
                checkInSheet.Cell(1, 7).Value = "Note";
                // Add other headers as needed

                int ciRow = 2;
                foreach (var ci in checkIns)
                {
                    checkInSheet.Cell(ciRow, 1).Value = ci.EmployeeId;
                    checkInSheet.Cell(ciRow, 2).Value = ci.Day;
                    checkInSheet.Cell(ciRow, 3).Value = ci.Month;
                    checkInSheet.Cell(ciRow, 4).Value = ci.Year;
                    checkInSheet.Cell(ciRow, 5).Value = ci.CheckInTime;
                    checkInSheet.Cell(ciRow, 6).Value = ci.Status.ToString();
                    checkInSheet.Cell(ciRow, 7).Value = ci.Note;
                    // Add other properties as needed
                    ciRow++;
                }

                // Export SalaryDetails
                var salaryDetails = await dbService.GetAllSalaryDetailsAsync();
                var salarySheet = workbook.Worksheets.Add("SalaryDetails");
                salarySheet.Cell(1, 1).Value = "EmployeeId";
                salarySheet.Cell(1, 2).Value = "Month";
                salarySheet.Cell(1, 3).Value = "Year";
                salarySheet.Cell(1, 4).Value = "BaseSalary";
                salarySheet.Cell(1, 5).Value = "DaysAbsent";
                salarySheet.Cell(1, 6).Value = "DaysOnLeave";
                salarySheet.Cell(1, 7).Value = "FinalSalary";
                // Add other headers as needed

                int salRow = 2;
                foreach (var sal in salaryDetails)
                {
                    salarySheet.Cell(salRow, 1).Value = sal.EmployeeId;
                    salarySheet.Cell(salRow, 2).Value = sal.Month;
                    salarySheet.Cell(salRow, 3).Value = sal.Year;
                    salarySheet.Cell(salRow, 4).Value = sal.BaseSalary;
                    salarySheet.Cell(salRow, 5).Value = sal.DaysAbsent;
                    salarySheet.Cell(salRow, 6).Value = sal.DaysOnLeave;
                    salarySheet.Cell(salRow, 7).Value = sal.FinalSalary;
                    // Add other properties as needed
                    salRow++;
                }

                // Save the workbook to the specified path
                workbook.SaveAs(filePath);
            }

            ShowLoadingScreen = false;

            await Shell.Current.DisplayAlert("Success", $"Data exported to {filePath} successfully.", "OK");
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to export Excel data: {ex.Message}", "OK");
        }
    }
}