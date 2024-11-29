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
}
