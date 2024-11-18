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

    public async Task DeleteSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        context.SalaryDetails.Remove(salaryDetail);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSalaryDetailAsync(SalaryDetail salaryDetail)
    {
        context.SalaryDetails.Update(salaryDetail);
        await context.SaveChangesAsync();
    }

    public async Task<SalaryDetail?> GetSalaryDetailByIdAsync(string id)
    {
        return await context.SalaryDetails.FindAsync(id);
    }

    public async Task<IEnumerable<SalaryDetail>> GetSalaryDetailsAsync()
    {
        return await context.SalaryDetails.ToListAsync();
    }
}
