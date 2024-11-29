namespace SandTetris.Interfaces;

public interface ISalaryService
{
    Task CalculateSalariesAsync(string departmentId, int month, int year);
    Task<decimal> GetEmployeeSalaryAsync(string employeeId, int month, int year);
    Task<decimal> GetDepartmentSalaryAsync(string departmentId, int month, int year);
}