using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;

namespace SandTetris.Data;

public class CheckInRepository(DataContext context) : ICheckInRepository
{
    public async Task AddCheckInAsync(CheckIn checkIn)
    {
        context.CheckIns.Add(checkIn);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCheckInAsync(CheckIn checkIn)
    {
        context.CheckIns.Remove(checkIn);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCheckInAsync(CheckIn checkIn)
    {
        context.CheckIns.Update(checkIn);
        await context.SaveChangesAsync();
    }

    public async Task<CheckIn?> GetCheckInByIdAsync(string id)
    {
        return await context.CheckIns.FindAsync(id);
    }

    public async Task<IEnumerable<CheckIn>> GetCheckInsAsync()
    {
        return await context.CheckIns.ToListAsync();
    }

    public async Task UpdateCheckInStatusAsync(DateTime day, string employeeId, CheckInStatus newStatus)
    {
        var checkIn = await context.CheckIns
                                     .FirstOrDefaultAsync(ci => ci.Day == day && ci.EmployeeId == employeeId);
        if (checkIn != null)
        {
            checkIn.Status = newStatus;
            await context.SaveChangesAsync();
        }
    }

}
