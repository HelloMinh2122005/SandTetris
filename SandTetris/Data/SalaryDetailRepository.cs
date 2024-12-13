using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Services;

namespace SandTetris.Data;

public class SalaryDetailRepository(DatabaseService databaseService) : ISalaryDetailRepository
{
    public async Task AddSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        databaseService.DataContext.SalaryDetails.Add(salaryDetail);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task UpdateSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        databaseService.DataContext.SalaryDetails.Update(salaryDetail);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<SalaryDetail>> GetSalaryDetailsAsync()
    {
        return await databaseService.DataContext.SalaryDetails
                                        .Include(sd => sd.Employee)
                                        .ToListAsync();
    }

    public async Task<IEnumerable<SalaryDetail>> GetSalaryDetailsMonthYearAsync(int month, int year)
    {
        var salaryDetails = await databaseService.DataContext.SalaryDetails
                                        .Include(sd => sd.Employee)
                                        .Where(sd => sd.Month == month && sd.Year == year)
                                        .ToListAsync();

        return salaryDetails
                .OrderByDescending(sd => sd.DaysWorking)
                .ThenByDescending(sd => sd.DaysOnLeave)
                .ThenByDescending(sd => sd.DaysAbsent);
    }

    public async Task AddDepositAsync(string employeeId, int month, int year, int amount)
    {
        var salaryDetail = await GetSalaryDetailAsync(employeeId, month, year);
        if (salaryDetail != null)
        {
            salaryDetail.FinalSalary += amount;
            salaryDetail.Deposit += amount;
            await databaseService.DataContext.SaveChangesAsync();
        }
    }

    public async Task RemoveDepositAsync(string employeeId, int month, int year)
    {
        var salaryDetail = await GetSalaryDetailAsync(employeeId, month, year);
        if (salaryDetail != null)
        {
            salaryDetail.FinalSalary -= salaryDetail.Deposit;
            salaryDetail.Deposit = 0;
            await databaseService.DataContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SalaryDetail>> GetTopEmployeeAsync()
    {
        var salaryDetails = await databaseService.DataContext.SalaryDetails
            .Include(sd => sd.Employee)
            .ToListAsync();

        return salaryDetails
            .Where(sd => sd.IsDeposited)
            .OrderBy(sd => sd.Year).ThenBy(sd => sd.Month);
    }


    public async Task<SalaryDetail?> GetSalaryDetailAsync(string employeeId, int month, int year)
    {
        return await databaseService.DataContext.SalaryDetails
                        .Include(sd => sd.Employee)
                        .FirstOrDefaultAsync(sd => sd.EmployeeId == employeeId && sd.Month == month && sd.Year == year);
    }

    public async Task DeleteSalaryDetailAsync(string employeeId, int month, int year)
    {
        var salaryDetail = await GetSalaryDetailAsync(employeeId, month, year);
        if (salaryDetail != null)
        {
            databaseService.DataContext.SalaryDetails.Remove(salaryDetail);
            await databaseService.DataContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SalaryDetail>> AddSalaryDetailsForDepartmentAsync(string departmentID, int month, int year)
    {
        var employees = await databaseService.DataContext.Employees
            .Where(e => e.DepartmentId == departmentID)
            .ToListAsync();

        foreach (var employee in employees)
        {
            var salaryDetail = new SalaryDetail
            {
                EmployeeId = employee.Id,
                Month = month,
                Year = year,
                BaseSalary = 0,
                DaysAbsent = 0,
                DaysOnLeave = 0
            };
            databaseService.DataContext.SalaryDetails.Add(salaryDetail);
        }

        await databaseService.DataContext.SaveChangesAsync();

        var salaryDetails = await databaseService.DataContext.SalaryDetails
            .Where(sd => sd.Employee.DepartmentId == departmentID && sd.Month == month && sd.Year == year)
            .Include(sd => sd.Employee)
            .ToListAsync();

        return salaryDetails;
    }

    public async Task<IEnumerable<SalaryDetail>> GetSalaryDetailsForDepartmentAsync(string departmentID, int month, int year)
    {
        var salaryDetails = await databaseService.DataContext.SalaryDetails
            .Where(sd => sd.Employee.DepartmentId == departmentID && sd.Month == month && sd.Year == year)
            .Include(sd => sd.Employee)
            .ToListAsync();
        return salaryDetails;
    }

    public async Task<IEnumerable<SalaryDetailSummary>> GetAllSalaryDetailSummariesAsync()
    {
        var summaries = await databaseService.DataContext.SalaryDetails
            .GroupBy(sd => new { sd.Month, sd.Year, sd.Employee.DepartmentId })
            .Select(g => new SalaryDetailSummary
            {
                DepartmentId = g.First().Employee.DepartmentId,
                DepartmentName = g.First().Employee.Department.Name,
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalSpent = g.Sum(sd => sd.FinalSalary)
            })
            .ToListAsync();

        return summaries;
    }
    public async Task<IEnumerable<SalaryDetailSummary>> GetSalaryDetailSummariesAsync(int month, int year)
    {
        var summaries = await databaseService.DataContext.SalaryDetails
            .Where(sd => sd.Month == month && sd.Year == year)
            .GroupBy(sd => new { sd.Month, sd.Year, sd.Employee.DepartmentId })
            .Select(g => new SalaryDetailSummary
            {
                DepartmentId = g.First().Employee.DepartmentId,
                DepartmentName = g.First().Employee.Department.Name,
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalSpent = g.Sum(sd => sd.FinalSalary)
            })
            .ToListAsync();

        return summaries;
    }

    public async Task<IEnumerable<SalaryDetailSummary>> AddSalaryDetailSummariesAsync(int month, int year)
    {
        var employees = await databaseService.DataContext.Employees
            .Include(e => e.Department)
            .ToListAsync();

        foreach (var employee in employees)
        {
            var salaryDetail = new SalaryDetail
            {
                EmployeeId = employee.Id,
                Month = month,
                Year = year,
                BaseSalary = 0,    
                DaysAbsent = 0,
                DaysOnLeave = 0,
                FinalSalary = 0
            };
            databaseService.DataContext.SalaryDetails.Add(salaryDetail);
        }

        await databaseService.DataContext.SaveChangesAsync();

        var summaries = await databaseService.DataContext.SalaryDetails
            .Where(sd => sd.Month == month && sd.Year == year)
            .GroupBy(sd => new { sd.Employee.DepartmentId, sd.Employee.Department.Name })
            .Select(g => new SalaryDetailSummary
            {
                DepartmentId = g.Key.DepartmentId,
                DepartmentName = g.Key.Name,
                Month = month,
                Year = year,
                TotalSpent = g.Sum(sd => sd.FinalSalary)
            })
            .ToListAsync();

        return summaries;
    }

    public async Task UpdateSalaryDetailSummariesAsync(string employeeID, int month, int year, int baseSalary, int dayAbsents, int dayOnleaves)
    {
        var salaryDetail = await databaseService.DataContext.SalaryDetails.FindAsync(employeeID, month, year);

        if (salaryDetail != null)
        {
            salaryDetail.BaseSalary = baseSalary;
            salaryDetail.DaysAbsent = dayAbsents;
            salaryDetail.DaysOnLeave = dayOnleaves;

            await databaseService.DataContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Ehe :))");
        }
    }
}