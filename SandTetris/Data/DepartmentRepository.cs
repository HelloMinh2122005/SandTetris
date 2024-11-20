using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Services;

namespace SandTetris.Data;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DataContext context;

    public DepartmentRepository(DatabaseService databaseService)
    {
        if (databaseService.DataContext == null)
        {
            throw new InvalidOperationException("DatabaseService is not initialized.");
        }

        context = databaseService.DataContext;
    }

    public async Task AddDepartmentAsync(Department department)
    {
        databaseService.DataContext.Departments.Add(department);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task DeleteDepartmentAsync(Department department)
    {
        databaseService.DataContext.Departments.Remove(department);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(string id)
    {
        return await databaseService.DataContext.Departments.FindAsync(id);
    }

    public async Task<IEnumerable<Department>> GetDepartmentsAsync()
    {
        return await databaseService.DataContext.Departments.ToListAsync();
    }

    public async Task UpdateDepartmentAsync(Department department)
    {
        databaseService.DataContext.Departments.Update(department);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task UpdateDeparmentHeadAsync(string departmentId, string employeeId)
    {
        var department = await context.Departments.FindAsync(departmentId);
        var employee = await context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        if (department == null)
        {
            throw new ArgumentException("Department not found");
        }
        if (employee.DepartmentId != departmentId)
        {
            throw new ArgumentException("Employee is not in this department");
        }
        department.HeadOfDepartment = employee;
        await databaseService.DataContext.SaveChangesAsync();
    }
}
