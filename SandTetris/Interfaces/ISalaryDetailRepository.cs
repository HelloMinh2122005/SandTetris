using SandTetris.Entities;

namespace SandTetris.Interfaces;

public interface ISalaryDetailRepository
{
    // Get 1 salary detail
    Task<SalaryDetail?> GetSalaryDetailAsync(string employeeId, int month, int year);

    // Get all salary details
    Task<IEnumerable<SalaryDetail>> GetSalaryDetailsAsync();

    // Add salary detail
    Task AddSalaryDetailAsync(SalaryDetail salaryDetail);

    // Update salary detail
    Task UpdateSalaryDetailAsync(SalaryDetail salaryDetail);

    // Delete salary detail
    Task DeleteSalaryDetailAsync(string employeeId, int month, int year);

    Task<IEnumerable<SalaryDetail>> AddSalaryDetailsForDepartmentAsync(string departmentID, int month, int year);
    Task<IEnumerable<SalaryDetail>> GetSalaryDetailsForDepartmentAsync(string departmentID, int month, int year);
    Task<IEnumerable<SalaryDetail>> GetSalaryDetailsForDepartmentAsync(string departmentID);
    Task<IEnumerable<SalaryDetailSummary>> GetAllSalaryDetailSummariesAsync();
    Task<IEnumerable<SalaryDetailSummary>> GetSalaryDetailSummariesAsync(int month, int year);
    Task<IEnumerable<SalaryDetailSummary>> AddSalaryDetailSummariesAsync(int month, int year);
    Task UpdateSalaryDetailSummariesAsync(string employeeID, int month, int year, int baseSalary, int dayAbsents, int dayOnleaves);
}
