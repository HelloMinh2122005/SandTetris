using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;

namespace SandTetris.Data;

public class SalaryDetailRepository(DataContext context) : ISalaryDetailRepository
{
    public async Task AddSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        context.SalaryDetails.Add(salaryDetail);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        context.SalaryDetails.Update(salaryDetail);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SalaryDetail>> GetSalaryDetailsAsync()
    {
        return await context.SalaryDetails.ToListAsync();
    }

    public async Task<SalaryDetail?> GetSalaryDetailAsync(string employeeId, int month, int year)
    {
        return await context.SalaryDetails.FindAsync(employeeId, month, year);
    }

    public async Task DeleteSalaryDetailAsync(string employeeId, int month, int year)
    {
        var salaryDetail = await GetSalaryDetailAsync(employeeId, month, year);
        if (salaryDetail != null)
        {
            context.SalaryDetails.Remove(salaryDetail);
            await context.SaveChangesAsync();
        }
    }
}
