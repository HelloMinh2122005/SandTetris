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
}
