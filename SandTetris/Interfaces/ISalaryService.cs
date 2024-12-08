namespace SandTetris.Interfaces;

public interface ISalaryService
{
    Task CalculateSalariesAsync(string departmentId, int month, int year);
    Task<int> CalculateSalaryForEmployeeAsync(string employeeId, int month, int year);
    Task<int> GetEmployeeSalaryAsync(string employeeId, int month, int year);
    Task<int> GetDepartmentSalaryAsync(string departmentId, int month, int year);
    Task<int> GetTotalSalaryAsync(int month, int year);
    Task<long> GetTotalAll();
}