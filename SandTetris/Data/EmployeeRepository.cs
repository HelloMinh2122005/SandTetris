using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Data;

public class EmployeeRepository(DatabaseService databaseService) : IEmployeeRepository
{
    public async Task AddEmployeeAsync(Employee employee)
    {
        databaseService.DataContext.Employees.Add(employee);
        await databaseService.DataContext.SaveChangesAsync();
    }
    public async Task DeleteEmployeeAsync(Employee employee)
    {
        databaseService.DataContext.Employees.Remove(employee);
        await databaseService.DataContext.SaveChangesAsync();
    }
    public async Task UpdateEmployeeAsync(Employee employee)
    {
        databaseService.DataContext.Employees.Update(employee);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(string id)
    {
        return await databaseService.DataContext.Employees.FindAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        return await databaseService.DataContext.Employees.ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string id)
    {
        return await databaseService.DataContext.Employees.Where(e => e.DepartmentId == id).ToListAsync();
    }

    public async Task UploadAvatarAsync(string employeeId, Stream imageStream, string fileExtension)
    {
        var employee = await databaseService.DataContext.Employees.FindAsync(employeeId);
        if (employee != null)
        {
            using (var memoryStream = new MemoryStream()) 
            {
                await imageStream.CopyToAsync(memoryStream);
                employee.Avatar = memoryStream.ToArray();
            }
            employee.AvatarFileExtension = fileExtension;
            await databaseService.DataContext.SaveChangesAsync();
        }
    }

    public async Task UpdateAvatarAsync(string employeeId, Stream imageStream, string fileExtension)
    {
        await UploadAvatarAsync(employeeId, imageStream, fileExtension);
    }

    public async Task<bool> CheckValidID(string id)
    {
        return !(await databaseService.DataContext.Employees.AnyAsync(e => e.Id == id));
    }
}
