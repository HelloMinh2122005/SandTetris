using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Services;

namespace SandTetris.Data;

public class CheckInRepository(DatabaseService databaseService) : ICheckInRepository
{
    public async Task AddCheckInAsync(CheckIn checkIn)
    {
        databaseService.DataContext.CheckIns.Add(checkIn);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task DeleteCheckInAsync(CheckIn checkIn)
    {
        databaseService.DataContext.CheckIns.Remove(checkIn);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task UpdateCheckInAsync(CheckIn checkIn)
    {
        databaseService.DataContext.CheckIns.Update(checkIn);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task<CheckIn?> GetCheckInByIdAsync(string id)
    {
        return await databaseService.DataContext.CheckIns.FindAsync(id);
    }

    public async Task<IEnumerable<CheckIn>> GetCheckInsAsync()
    {
        return await databaseService.DataContext.CheckIns.ToListAsync();
    }

    public async Task UpdateCheckInStatusAsync(DateTime day, string employeeId, CheckInStatus newStatus)
    {
        var checkIn = await databaseService.DataContext.CheckIns
                                     .FirstOrDefaultAsync(ci => ci.Day == day && ci.EmployeeId == employeeId);
        if (checkIn != null)
        {
            checkIn.Status = newStatus;
            await databaseService.DataContext.SaveChangesAsync();
        }
    }

}
