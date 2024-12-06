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

    public async Task<IEnumerable<CheckIn>> GetCheckInsAsync()
    {
        return await databaseService.DataContext.CheckIns.ToListAsync();
    }

    public async Task AddCheckInsForDepartmentAsync(string departmentId, int day, int month, int year)
    {
        var employees = await databaseService.DataContext.Employees
            .Where(e => e.DepartmentId == departmentId)
            .ToListAsync();

        foreach (var employee in employees)
        {
            bool exists = await databaseService.DataContext.CheckIns
                .AnyAsync(ci => ci.EmployeeId == employee.Id && ci.Day == day && ci.Month == month && ci.Year == year);

            if (!exists)
            {
                var checkIn = new CheckIn
                {
                    EmployeeId = employee.Id,
                    Day = day,
                    Month = month,
                    Year = year,
                    Status = CheckInStatus.Absent // Default status
                };

                databaseService.DataContext.CheckIns.Add(checkIn);
            }
        }

        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task DeleteCheckInForDepartmentAsync(string departmentId, int day, int month, int year)
    {
        var checkIns = await databaseService.DataContext.CheckIns
            .Where(ci => ci.Employee.DepartmentId == departmentId && ci.Day == day && ci.Month == month && ci.Year == year)
            .ToListAsync();
        databaseService.DataContext.CheckIns.RemoveRange(checkIns);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CheckIn>> GetCheckInsForDepartmentAsync(string departmentId, int day, int month, int year)
    {
        return await databaseService.DataContext.CheckIns
            .Where(ci => ci.Employee.DepartmentId == departmentId && ci.Day == day && ci.Month == month && ci.Year == year)
            .ToListAsync();
    }

    public async Task<IEnumerable<CheckInSummary>> GetAllCheckInSummariesAsync(string departmentId)
    {
        var summaries = await databaseService.DataContext.CheckIns
            .Where(ci => ci.Employee.DepartmentId == departmentId)
            .GroupBy(ci => new { ci.Day, ci.Month, ci.Year })
            .Select(g => new CheckInSummary
            {
                Day = g.Key.Day,
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalWorking = g.Count(ci => ci.Status == CheckInStatus.Working),
                TotalOnLeave = g.Count(ci => ci.Status == CheckInStatus.OnLeave),
                TotalAbsent = g.Count(ci => ci.Status == CheckInStatus.Absent)
            })
            .OrderByDescending(s => s.Year).OrderByDescending(s => s.Month).OrderByDescending(s => s.Day)
            .ToListAsync();

        return summaries;
    }

    public async Task<IEnumerable<CheckInSummary>> GetCheckInSummariesAsync(string departmentId, int month, int year)
    {
        var summaries = await databaseService.DataContext.CheckIns
            .Where(ci => ci.Employee.DepartmentId == departmentId && ci.Month == month && ci.Year == year)
            .GroupBy(ci => new { ci.Day, ci.Month, ci.Year })
            .Select(g => new CheckInSummary
            {
                Day = g.Key.Day,
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalWorking = g.Count(ci => ci.Status == CheckInStatus.Working),
                TotalOnLeave = g.Count(ci => ci.Status == CheckInStatus.OnLeave),
                TotalAbsent = g.Count(ci => ci.Status == CheckInStatus.Absent)
            })
            .OrderBy(s => s.Year).ThenBy(s => s.Month).ThenBy(s => s.Day)
            .ToListAsync();

        return summaries;
    }

    public async Task UpdateEmployeeCheckInAsync(string employeeId, int day, int month, int year, CheckInStatus status, DateTime checkInTime, string note)
    {
        var checkIn = await databaseService.DataContext.CheckIns.FindAsync(employeeId, day, month, year);

        if (checkIn != null)
        {
            checkIn.Status = status;
            checkIn.CheckInTime = checkInTime;
            checkIn.Note = note;
            await databaseService.DataContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("How the fuck did you even get here ?");
        }
    }

    public async Task<IEnumerable<CheckIn>> GetCheckInsForEmployeeAsync(string employeeId, int month, int year)
    {
        return await databaseService.DataContext.CheckIns
                                    .Where(ci => ci.EmployeeId == employeeId && ci.Month == month && ci.Year == year)
                                    .ToListAsync();
    }

    public async Task<CheckIn?> GetCheckInByIdAsync(string employeeId, int day, int month, int year)
    {
        return await databaseService.DataContext.CheckIns
        .FirstOrDefaultAsync(ci => ci.EmployeeId == employeeId && ci.Day == day && ci.Month == month && ci.Year == year);
    }

    public async Task UpdateCheckInSummary(string departmentId, int day, int month, int year, CheckInStatus newStatus, CheckInStatus preStatus)
    {
        var checkInSummary = await databaseService.DataContext.CheckIns
            .Where(ci => ci.Employee.DepartmentId == departmentId && ci.Day == day && ci.Month == month && ci.Year == year)
            .GroupBy(ci => new { ci.Day, ci.Month, ci.Year })
            .Select(g => new CheckInSummary
            {
                Day = g.Key.Day,
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalWorking = g.Count(ci => ci.Status == CheckInStatus.Working),
                TotalOnLeave = g.Count(ci => ci.Status == CheckInStatus.OnLeave),
                TotalAbsent = g.Count(ci => ci.Status == CheckInStatus.Absent)
            })
            .FirstOrDefaultAsync();

        if (checkInSummary != null)
        {
            if (preStatus == CheckInStatus.Working)
            {
                checkInSummary.TotalWorking--;
            }
            else if (preStatus == CheckInStatus.OnLeave)
            {
                checkInSummary.TotalOnLeave--;
            }
            else if (preStatus == CheckInStatus.Absent)
            {
                checkInSummary.TotalAbsent--;
            }

            if (newStatus == CheckInStatus.Working)
            {
                checkInSummary.TotalWorking++;
            }
            else if (newStatus == CheckInStatus.OnLeave)
            {
                checkInSummary.TotalOnLeave++;
            }
            else if (newStatus == CheckInStatus.Absent)
            {
                checkInSummary.TotalAbsent++;
            }

            await databaseService.DataContext.SaveChangesAsync();
        }
    }

    public async Task<bool> CheckValidDayAsync(string departmenId, int day, int month, int year)
    {
        return !(await databaseService.DataContext.CheckIns
            .AnyAsync(ci => ci.Employee.DepartmentId == departmenId && ci.Day == day && ci.Month == month && ci.Year == year));
    }
}
