using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;
using SandTetris.Services;

namespace SandTetris.Data;

public class DepartmentRepository(DatabaseService databaseService) : IDepartmentRepository
{
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
        return await databaseService.DataContext.Departments.Include(d => d.Employees).ToListAsync();
    }

    public async Task UpdateDepartmentAsync(Department department)
    {
        databaseService.DataContext.Departments.Update(department);
        await databaseService.DataContext.SaveChangesAsync();
    }

    public async Task UpdateDeparmentHeadAsync(string departmentId, string employeeId)
    {
        var department = await databaseService.DataContext.Departments.FindAsync(departmentId);
        var employee = await databaseService.DataContext.Employees.FindAsync(employeeId);
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

    public async Task<Employee?> GetDepartmentHeadAsync(string departmentId)
    {
        var department = await databaseService.DataContext.Departments.FindAsync(departmentId) ?? throw new ArgumentException("Department not found");
        if (department.HeadOfDepartmentId == null)
        {
            throw new ArgumentException("Department head not found");
        }
        return await databaseService.DataContext.Employees.FindAsync(department.HeadOfDepartmentId);
    }

    public async Task<int> GetTotalDepartmentEmployees(string departmentId)
    {
        return await databaseService.DataContext.Employees.CountAsync(e => e.DepartmentId == departmentId);
    }


}
