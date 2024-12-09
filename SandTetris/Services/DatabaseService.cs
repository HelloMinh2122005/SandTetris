using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandTetris.Data;
using SandTetris.Entities;

namespace SandTetris.Services;

public class DatabaseService
{
    private SqliteConnection? sqliteConnection;
    private DataContext? dataContext;

    public DataContext DataContext => dataContext ?? throw new ArgumentNullException("Database not initialized!");

    public async Task Initialize(string dbPath)
    {
        try
        {
            sqliteConnection = new SqliteConnection($"Data Source={dbPath}");
            await sqliteConnection.OpenAsync();

            var dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(sqliteConnection)
                .Options;

            dataContext = new DataContext(dbOptions);
            await dataContext.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new Exception($"Database initialization failed: {ex.Message}", ex);
        }
    }
    public async Task AddOrUpdateEmployeeAsync(Employee employee)
    {
        var existingEmployee = await dataContext.Employees
            .FirstOrDefaultAsync(e => e.Id == employee.Id);

        if (existingEmployee != null)
        {
            // Update existing employee
            existingEmployee.FullName = employee.FullName;
            existingEmployee.DoB = employee.DoB;
            existingEmployee.Title = employee.Title;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.Avatar = employee.Avatar;
            existingEmployee.AvatarFileExtension = employee.AvatarFileExtension;
            // Update other properties as needed
        }
        else
        {
            // Add new employee
            await dataContext.Employees.AddAsync(employee);
        }

        await dataContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateDepartmentAsync(Department department)
    {
        var existingDepartment = await dataContext.Departments
            .FirstOrDefaultAsync(d => d.Id == department.Id);

        if (existingDepartment != null)
        {
            // Update existing department
            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;
            existingDepartment.HeadOfDepartmentId = department.HeadOfDepartmentId;
            // Update other properties as needed
        }
        else
        {
            // Add new department
            await dataContext.Departments.AddAsync(department);
        }

        await dataContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateCheckInAsync(CheckIn checkIn)
    {
        var existingCheckIn = await dataContext.CheckIns
            .FirstOrDefaultAsync(ci => ci.EmployeeId == checkIn.EmployeeId
                                    && ci.Day == checkIn.Day
                                    && ci.Month == checkIn.Month
                                    && ci.Year == checkIn.Year);

        if (existingCheckIn != null)
        {
            // Update existing CheckIn
            existingCheckIn.CheckInTime = checkIn.CheckInTime;
            existingCheckIn.Status = checkIn.Status;
            existingCheckIn.Note = checkIn.Note;
            // Update other properties as needed
        }
        else
        {
            // Add new CheckIn
            await dataContext.CheckIns.AddAsync(checkIn);
        }

        await dataContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        var existingSalaryDetail = await dataContext.SalaryDetails
            .FirstOrDefaultAsync(sd => sd.EmployeeId == salaryDetail.EmployeeId
                                   && sd.Month == salaryDetail.Month
                                   && sd.Year == salaryDetail.Year);

        if (existingSalaryDetail != null)
        {
            // Update existing SalaryDetail
            existingSalaryDetail.BaseSalary = salaryDetail.BaseSalary;
            existingSalaryDetail.DaysAbsent = salaryDetail.DaysAbsent;
            existingSalaryDetail.DaysOnLeave = salaryDetail.DaysOnLeave;
            existingSalaryDetail.FinalSalary = salaryDetail.FinalSalary;
            // Update other properties as needed
        }
        else
        {
            // Add new SalaryDetail
            await dataContext.SalaryDetails.AddAsync(salaryDetail);
        }

        await dataContext.SaveChangesAsync();
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await dataContext.Employees.ToListAsync();
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        return await dataContext.Departments.ToListAsync();
    }

    public async Task<List<CheckIn>> GetAllCheckInsAsync()
    {
        return await dataContext.CheckIns.ToListAsync();
    }

    public async Task<List<SalaryDetail>> GetAllSalaryDetailsAsync()
    {
        return await dataContext.SalaryDetails.ToListAsync();
    }
}