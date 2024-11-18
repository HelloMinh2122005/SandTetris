using SandTetris.Entities;

namespace SandTetris.Interfaces;

public interface ISalaryDetailRepository
{
    //Get 1 salary detail
    Task<SalaryDetail?> GetSalaryDetailByIdAsync(string id);
    //Get all salary details
    Task<IEnumerable<SalaryDetail>> GetSalaryDetailsAsync();
    //Add salary detail
    Task AddSalaryDetailAsync(SalaryDetail salaryDetail);
    //Update salary detail
    Task UpdateSalaryDetailAsync(SalaryDetail salaryDetail);
    //Delete salary detail
    Task DeleteSalaryDetailAsync(SalaryDetail salaryDetail);
}
