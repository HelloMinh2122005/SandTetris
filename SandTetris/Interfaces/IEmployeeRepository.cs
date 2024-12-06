using SandTetris.Entities;

namespace SandTetris.Interfaces;

public interface IEmployeeRepository
{
    //Get 1 employee
    Task<Employee?> GetEmployeeByIdAsync(string id);
    //Get all employees
    Task<IEnumerable<Employee>> GetEmployeesAsync();
    //Get all employees by department
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string id);
    //Add employee
    Task AddEmployeeAsync(Employee employee);
    //Update employee
    Task UpdateEmployeeAsync(Employee employee);
    //Delete employee
    Task DeleteEmployeeAsync(Employee employee);
    //Upload avatar
    Task UploadAvatarAsync(string employeeId, Stream imageStream, string fileExtension);
    //Update avatar
    Task UpdateAvatarAsync(string employeeId, Stream imageStream, string fileExtension);
    Task<bool> CheckValidID(string id);
}
