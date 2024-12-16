using ClosedXML.Excel;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

    [ObservableProperty]
    private bool isLogined = false;

    [ObservableProperty]
    private bool isNotLogined = true;

    [ObservableProperty]
    private string loginUsername = "";

    [ObservableProperty]
    private string loginPassword = "";

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

    public async Task<bool> LoginAsync()
    {
        if (string.IsNullOrEmpty(LoginUsername) || string.IsNullOrEmpty(LoginPassword))
        {
            await Shell.Current.DisplayAlert("Error", "Username and password are required.", "OK");
            return false;
        }
        if (LoginUsername == "SandTetris" && LoginPassword == "sandtetris123")
        {
            IsLogined = true;
            IsNotLogined = false;
            return true;
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Wrong username or password.", "OK");
            return false;
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
                    ShowLoadingScreen = false;
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

                using (var workbook = new XLWorkbook(excelFilePath))
                using (var transaction = await dbService.DataContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Validate Excel data before importing
                        await ValidateExcelDataAsync(workbook);

                        // Clear existing data before importing
                        await ClearExistingDataAsync();

                        // Import Departments without HeadOfDepartmentId
                        await ImportDepartmentsAsync(workbook, setHeadOfDepartment: false);

                        // Save changes to ensure Departments are persisted before importing Employees
                        await dbService.DataContext.SaveChangesAsync();

                        // Import Employees
                        await ImportEmployeesAsync(workbook);

                        // Now update Departments with HeadOfDepartmentId
                        await ImportDepartmentsAsync(workbook, setHeadOfDepartment: true);

                        // Save changes after updating Departments
                        await dbService.DataContext.SaveChangesAsync();

                        // Import CheckIns
                        await ImportCheckInsAsync(workbook);

                        // Import SalaryDetails
                        await ImportSalaryDetailsAsync(workbook);

                        // Commit the transaction after all imports are successful
                        await transaction.CommitAsync();
                        await Shell.Current.DisplayAlert("Success", "Excel data imported successfully.", "OK");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        string errorMessage = ex.InnerException?.Message ?? ex.Message;
                        Console.WriteLine($"Import Error: {errorMessage}");
                        await Shell.Current.DisplayAlert("Error", $"Failed to import Excel data: {errorMessage}", "OK");
                    }
                }

                ShowLoadingScreen = false;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Selected Excel file does not exist.", "OK");
                ShowLoadingScreen = false;
            }
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
                try
                {
                    string employeeId = row.Cell(1).GetString().Trim();
                    string fullName = row.Cell(2).GetString().Trim();
                    DateTime doB = row.Cell(3).GetDateTime();
                    string title = row.Cell(4).GetString().Trim();
                    string departmentId = row.Cell(5).GetString().Trim();

                    // Validate DepartmentId
                    bool departmentExists = await dbService.DataContext.Departments
                        .AnyAsync(d => d.Id == departmentId);

                    if (!departmentExists)
                    {
                        throw new Exception($"Employee {fullName} references non-existent DepartmentId '{departmentId}'.");
                    }

                    // Handle Avatar Path from Excel
                    string avatarPath = row.Cell(6).GetString().Trim(); // Assuming AvatarPath is in column 6

                    byte[]? avatarBytes = null;
                    string? avatarExtension = null;

                    if (!string.IsNullOrEmpty(avatarPath))
                    {
                        if (File.Exists(avatarPath))
                        {
                            try
                            {
                                avatarBytes = await File.ReadAllBytesAsync(avatarPath);
                                avatarExtension = Path.GetExtension(avatarPath);
                            }
                            catch (Exception avatarEx)
                            {
                                // Log the avatar read error and continue without avatar
                                Console.WriteLine($"Warning: Failed to read avatar for Employee '{fullName}' at '{avatarPath}'. Error: {avatarEx.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Avatar file does not exist at '{avatarPath}' for Employee '{fullName}'.");
                        }
                    }

                    var employee = new Employee
                    {
                        Id = employeeId,
                        FullName = fullName,
                        DoB = doB,
                        Title = title,
                        DepartmentId = departmentId,
                        Avatar = avatarBytes,
                        AvatarFileExtension = avatarExtension
                    };

                    // Add or update employee in the database
                    await dbService.AddOrUpdateEmployeeAsync(employee);
                }
                catch (Exception ex)
                {
                    // Log the error and continue with the next employee
                    Console.WriteLine($"Error importing employee: {ex.Message}");
                    // Optionally, you can collect errors to display after the import
                }
            }
        }
    }

    private async Task ImportDepartmentsAsync(XLWorkbook workbook, bool setHeadOfDepartment)
    {
        if (workbook.Worksheets.Contains("Departments"))
        {
            var worksheet = workbook.Worksheet("Departments");
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                try
                {
                    string deptId = row.Cell(1).GetString().Trim();
                    string name = row.Cell(2).GetString().Trim();
                    string description = row.Cell(3).GetString().Trim();
                    string headIdRaw = setHeadOfDepartment ? row.Cell(4).GetString().Trim() : null;
                    string? headOfDepartmentId = string.IsNullOrEmpty(headIdRaw) ? null : headIdRaw;

                    // Validate HeadOfDepartmentId if set
                    if (setHeadOfDepartment && !string.IsNullOrEmpty(headOfDepartmentId))
                    {
                        bool employeeExists = await dbService.DataContext.Employees
                            .AnyAsync(e => e.Id == headOfDepartmentId);

                        if (!employeeExists)
                        {
                            throw new Exception($"HeadOfDepartmentId '{headOfDepartmentId}' does not exist for Department '{name}'.");
                        }
                    }

                    var department = new Department
                    {
                        Id = deptId,
                        Name = name,
                        Description = description,
                        HeadOfDepartmentId = headOfDepartmentId
                    };

                    // Add or update department in the database
                    await dbService.AddOrUpdateDepartmentAsync(department);
                }
                catch (Exception ex)
                {
                    // Log the error and continue with the next department
                    Console.WriteLine($"Error importing department: {ex.Message}");
                    // Optionally, you can collect errors to display after the import
                }
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
                    Status = Enum.Parse<CheckInStatus>(row.Cell(5).GetString()),
                    CheckInTime = row.Cell(6).GetDateTime(),
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
                    Deposit = row.Cell(5).GetValue<int>(),
                    DaysAbsent = row.Cell(6).GetValue<int>(),
                    DaysOnLeave = row.Cell(7).GetValue<int>(),
                    FinalSalary = row.Cell(8).GetValue<int>()
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
            ShowLoadingScreen = true;

            var result = await FolderPicker.Default.PickAsync();

            if (result != null && result.Folder != null && !string.IsNullOrEmpty(result.Folder.Path))
            {
                var folderPath = result.Folder.Path;
                var avatarsDir = Path.Combine(folderPath, "Avatars");
                if (!Directory.Exists(avatarsDir))
                {
                    Directory.CreateDirectory(avatarsDir);
                }

                // Define the Excel file name
                string fileName = $"SandTetris.xlsx";
                string filePath = Path.Combine(folderPath, fileName);

                using (var workbook = new XLWorkbook())
                {
                    // Export Employees
                    var employees = await dbService.GetAllEmployeesAsync();
                    var empSheet = workbook.Worksheets.Add("Employees");
                    empSheet.Cell(1, 1).Value = "Mã nhân viên";
                    empSheet.Cell(1, 2).Value = "Họ và tên";
                    empSheet.Cell(1, 3).Value = "Ngày tháng năm sinh";
                    empSheet.Cell(1, 4).Value = "Chức vụ";
                    empSheet.Cell(1, 5).Value = "Mã phòng ban";
                    // Add other headers as needed

                    int empRow = 2;
                    foreach (var emp in employees)
                    {
                        empSheet.Cell(empRow, 1).Value = emp.Id;
                        empSheet.Cell(empRow, 2).Value = emp.FullName;
                        empSheet.Cell(empRow, 3).Value = emp.DoB;
                        empSheet.Cell(empRow, 4).Value = emp.Title;
                        empSheet.Cell(empRow, 5).Value = emp.DepartmentId;

                        string avatarPath = "";
                        if (emp.Avatar != null && emp.Avatar.Length > 0)
                        {
                            string avatarFileName = $"{emp.Id}{emp.AvatarFileExtension ?? "jpg"}"; // Default to .jpg
                            string fullAvatarPath = Path.Combine(avatarsDir, avatarFileName);

                            // Save the avatar byte array as an image file
                            await File.WriteAllBytesAsync(fullAvatarPath, emp.Avatar);

                            // Set the relative or absolute path as needed
                            avatarPath = fullAvatarPath;
                        }
                        empSheet.Cell(empRow, 6).Value = avatarPath;
                        empRow++;
                    }

                    // Export Departments
                    var departments = await dbService.GetAllDepartmentsAsync();
                    var deptSheet = workbook.Worksheets.Add("Departments");
                    deptSheet.Cell(1, 1).Value = "Mã phòng ban";
                    deptSheet.Cell(1, 2).Value = "Tên phòng ban";
                    deptSheet.Cell(1, 3).Value = "Mô tả";
                    deptSheet.Cell(1, 4).Value = "Mã trưởng phòng";

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
                    checkInSheet.Cell(1, 1).Value = "Mã nhân viên";
                    checkInSheet.Cell(1, 2).Value = "Ngày";
                    checkInSheet.Cell(1, 3).Value = "Tháng";
                    checkInSheet.Cell(1, 4).Value = "Năm";
                    checkInSheet.Cell(1, 5).Value = "Trạng thái";
                    checkInSheet.Cell(1, 6).Value = "Thời gian điểm danh";
                    checkInSheet.Cell(1, 7).Value = "Ghi chú";
                    // Add other headers as needed

                    int ciRow = 2;
                    foreach (var ci in checkIns)
                    {
                        checkInSheet.Cell(ciRow, 1).Value = ci.EmployeeId;
                        checkInSheet.Cell(ciRow, 2).Value = ci.Day;
                        checkInSheet.Cell(ciRow, 3).Value = ci.Month;
                        checkInSheet.Cell(ciRow, 4).Value = ci.Year;
                        checkInSheet.Cell(ciRow, 5).Value = ci.Status.ToString();
                        checkInSheet.Cell(ciRow, 6).Value = ci.CheckInTime;
                        checkInSheet.Cell(ciRow, 7).Value = ci.Note;
                        // Add other properties as needed
                        ciRow++;
                    }

                    // Export SalaryDetails
                    var salaryDetails = await dbService.GetAllSalaryDetailsAsync();
                    var salarySheet = workbook.Worksheets.Add("SalaryDetails");
                    salarySheet.Cell(1, 1).Value = "Mã nhân viên";
                    salarySheet.Cell(1, 2).Value = "Tháng";
                    salarySheet.Cell(1, 3).Value = "Năm";
                    salarySheet.Cell(1, 4).Value = "Lương cơ sở";
                    salarySheet.Cell(1, 5).Value = "Thưởng/phạt"; // Added Deposit column header
                    salarySheet.Cell(1, 6).Value = "Ngày vắng";
                    salarySheet.Cell(1, 7).Value = "Ngày nghỉ";
                    salarySheet.Cell(1, 8).Value = "Lương cuối cùng";

                    int salRow = 2;
                    foreach (var sal in salaryDetails)
                    {
                        salarySheet.Cell(salRow, 1).Value = sal.EmployeeId;
                        salarySheet.Cell(salRow, 2).Value = sal.Month;
                        salarySheet.Cell(salRow, 3).Value = sal.Year;
                        salarySheet.Cell(salRow, 4).Value = sal.BaseSalary;
                        salarySheet.Cell(salRow, 5).Value = sal.Deposit; // Export Deposit value
                        salarySheet.Cell(salRow, 6).Value = sal.DaysAbsent;
                        salarySheet.Cell(salRow, 7).Value = sal.DaysOnLeave;
                        salarySheet.Cell(salRow, 8).Value = sal.FinalSalary;
                        salRow++;
                    }
                    empSheet.Columns().AdjustToContents();
                    deptSheet.Columns().AdjustToContents();
                    checkInSheet.Columns().AdjustToContents();
                    salarySheet.Columns().AdjustToContents();
                    // Save the workbook to the specified path
                    workbook.SaveAs(filePath);
                }

                ShowLoadingScreen = false;

                await Shell.Current.DisplayAlert("Success", $"Data exported to {filePath} successfully.", "OK");
            }
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to export Excel data: {ex.Message}", "OK");
        }
    }

    private async Task ValidateExcelDataAsync(XLWorkbook workbook)
    {
        // Validate Departments
        if (workbook.Worksheets.Contains("Departments"))
        {
            var deptSheet = workbook.Worksheet("Departments");
            var deptRows = deptSheet.RangeUsed().RowsUsed().Skip(1); // Skip header
            var departmentIds = new HashSet<string>();

            foreach (var row in deptRows)
            {
                string deptId = row.Cell(1).GetString().Trim();
                if (string.IsNullOrEmpty(deptId))
                {
                    throw new Exception("Department ID cannot be empty.");
                }

                if (!departmentIds.Add(deptId))
                {
                    throw new Exception($"Duplicate Department ID found: {deptId}");
                }

                string? headId = row.Cell(4).GetString().Trim();
                if (!string.IsNullOrEmpty(headId))
                {
                    // Will validate after importing Employees
                }
            }
        }

        // Validate Employees
        if (workbook.Worksheets.Contains("Employees"))
        {
            var empSheet = workbook.Worksheet("Employees");
            var empRows = empSheet.RangeUsed().RowsUsed().Skip(1); // Skip header
            var employeeIds = new HashSet<string>();

            foreach (var row in empRows)
            {
                string empId = row.Cell(1).GetString().Trim();
                if (string.IsNullOrEmpty(empId))
                {
                    throw new Exception("Employee ID cannot be empty.");
                }

                if (!employeeIds.Add(empId))
                {
                    throw new Exception($"Duplicate Employee ID found: {empId}");
                }

                string deptId = row.Cell(5).GetString().Trim();
                if (string.IsNullOrEmpty(deptId))
                {
                    throw new Exception($"Employee {empId} has empty DepartmentId.");
                }

                // Check if DepartmentId exists
                bool deptExists = await dbService.DataContext.Departments
                    .AnyAsync(d => d.Id == deptId);
            }
        }
    }
    [RelayCommand]
    public async Task ExportDbAsync()
    {
        try
        {
            ShowLoadingScreen = true;

            var result = await FolderPicker.Default.PickAsync();

            if (result != null && result.Folder != null && !string.IsNullOrEmpty(result.Folder.Path))
            {
                var folderPath = result.Folder.Path;

                // Get the current database path from preferences
                var dbPath = Preferences.Get("DBPATH", "");

                if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
                {
                    await Shell.Current.DisplayAlert("Error", "Database file not found.", "OK");
                    ShowLoadingScreen = false;
                    return;
                }

                // Define the destination file path
                var fileName = Path.GetFileName(dbPath);
                var destinationPath = Path.Combine(folderPath, fileName);

                // Copy the database file to the selected folder
                File.Copy(dbPath, destinationPath, overwrite: true);

                ShowLoadingScreen = false;

                await Shell.Current.DisplayAlert("Success", $"Database exported to {destinationPath}", "OK");
            }
            else
            {
                ShowLoadingScreen = false;
                await Shell.Current.DisplayAlert("Cancelled", "Export operation was cancelled.", "OK");
            }
        }
        catch (Exception ex)
        {
            ShowLoadingScreen = false;
            await Shell.Current.DisplayAlert("Error", $"Failed to export database: {ex.Message}", "OK");
        }
    }

    private async Task ClearExistingDataAsync()
    {
        // Remove data from child tables first to avoid foreign key constraints
        dbService.DataContext.CheckIns.RemoveRange(dbService.DataContext.CheckIns);
        dbService.DataContext.SalaryDetails.RemoveRange(dbService.DataContext.SalaryDetails);
        dbService.DataContext.Employees.RemoveRange(dbService.DataContext.Employees);
        dbService.DataContext.Departments.RemoveRange(dbService.DataContext.Departments);

        await dbService.DataContext.SaveChangesAsync();
    }
}