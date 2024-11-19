using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Data;

public class EmployeeRepository(DataContext context) : IEmployeeRepository
{
    public async Task AddEmployeeAsync(Employee employee)
    {
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
    }
    public async Task DeleteEmployeeAsync(Employee employee)
    {
        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
    }
    public async Task UpdateEmployeeAsync(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(string id)
    {
        return await context.Employees.FindAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        return await context.Employees.ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string id)
    {
        return await context.Employees.Where(e => e.DepartmentId == id).ToListAsync();
    }

    public async Task UploadAvatarAsync(string employeeId, Stream imageStream, string fileExtension)
    {
        var employee = await context.Employees.FindAsync(employeeId);
        if (employee != null)
        {
            using (var memoryStream = new MemoryStream()) 
            {
                await imageStream.CopyToAsync(memoryStream);
                employee.Avatar = memoryStream.ToArray();
            }
            employee.AvatarFileExtension = fileExtension;
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAvatarAsync(string employeeId, Stream imageStream, string fileExtension)
    {
        await UploadAvatarAsync(employeeId, imageStream, fileExtension);
    }
}
