using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;
using SandTetris.Interfaces;

namespace SandTetris.Data;

public class DepartmentRepository(DataContext context) : IDepartmentRepository
{
    public async Task AddDepartmentAsync(Department department)
    {
        context.Departments.Add(department);
        await context.SaveChangesAsync();
    }

    public async Task DeleteDepartmentAsync(Department department)
    {
        context.Departments.Remove(department);
        await context.SaveChangesAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(string id)
    {
        return await context.Departments.FindAsync(id);
    }

    public async Task<IEnumerable<Department>> GetDepartmentsAsync()
    {
        return await context.Departments.ToListAsync();
    }

    public async Task UpdateDepartmentAsync(Department department)
    {
        context.Departments.Update(department);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDeparmentHeadAsync(string departmentId, string employeeId)
    {
        var department = await context.Departments.FindAsync(departmentId);
        var employee = await context.Employees.FindAsync(employeeId);
        if (employee == null) { 
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
        await context.SaveChangesAsync();
    }
}
