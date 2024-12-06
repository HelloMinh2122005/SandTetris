using SandTetris.Entities;

namespace SandTetris.Interfaces;

public interface IDepartmentRepository
{
    //Get 1 department
    Task<Department?> GetDepartmentByIdAsync(string id);
    //Get all departments
    Task<IEnumerable<Department>> GetDepartmentsAsync();
    Task<Employee?> GetDepartmentHeadAsync(string departmentId);
    //Add department
    Task AddDepartmentAsync(Department department);
    //Update department
    Task UpdateDepartmentAsync(Department department);
    //Delete department
    Task DeleteDepartmentAsync(Department department);
    Task<int> GetTotalDepartmentEmployees(string departmentId);
    Task UpdateDeparmentHeadAsync(string departmentId, string employeeId);
    Task<bool> CheckValidID(string id);
}
