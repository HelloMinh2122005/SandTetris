using System;
using SandTetris.Entities;
using SandTetris.Interfaces;

namespace SandTetris.Services;

public class SalaryService(ICheckInRepository checkInRepository,
    ISalaryDetailRepository salaryDetailRepository, IEmployeeRepository employeeRepository) : ISalaryService
{
    public async Task CalculateSalariesAsync(string departmentId, int month, int year)
    {
        var employees = await employeeRepository.GetEmployeesByDepartmentAsync(departmentId);

        foreach (var employee in employees)
        {
            // Fetch existing SalaryDetail
            var existingSalaryDetail = await salaryDetailRepository.GetSalaryDetailAsync(employee.Id, month, year);

            // Determine BaseSalary
            if (existingSalaryDetail == null) throw new Exception("Base salary not found");
            decimal baseSalary = existingSalaryDetail.BaseSalary;

            // Fetch check-ins for the employee
            var checkIns = await checkInRepository.GetCheckInsForEmployeeAsync(employee.Id, month, year);
            int daysAbsent = checkIns.Count(ci => ci.Status == CheckInStatus.Absent);
            int daysOnLeave = checkIns.Count(ci => ci.Status == CheckInStatus.OnLeave);

            decimal finalSalary = baseSalary;

            // Apply business rules
            if (daysAbsent + daysOnLeave > 10)
            {
                finalSalary = 0;
            }
            else
            {
                finalSalary -= baseSalary * 0.03m * daysAbsent;
            }

            // Prepare SalaryDetail
            var salaryDetail = new SalaryDetail
            {
                EmployeeId = employee.Id,
                Month = month,
                Year = year,
                BaseSalary = baseSalary,
                DaysAbsent = daysAbsent,
                DaysOnLeave = daysOnLeave,
                FinalSalary = finalSalary
            };

            if (existingSalaryDetail != null)
            {
                // Update existing SalaryDetail
                existingSalaryDetail.DaysAbsent = daysAbsent;
                existingSalaryDetail.DaysOnLeave = daysOnLeave;
                existingSalaryDetail.FinalSalary = finalSalary;
                existingSalaryDetail.BaseSalary = baseSalary; // Update in case it has changed
                await salaryDetailRepository.UpdateSalaryDetailAsync(existingSalaryDetail);
            }
            else
            {
                // Add new SalaryDetail
                await salaryDetailRepository.AddSalaryDetailAsync(salaryDetail);
            }
        }
    }

    public async Task<decimal> GetDepartmentSalaryAsync(string departmentId, int month, int year)
    {
        var salaryDetails = await salaryDetailRepository.GetSalaryDetailsAsync();
        var departmentSalary = salaryDetails
            .Where(sd => sd.Employee.DepartmentId == departmentId && sd.Month == month && sd.Year == year)
            .Sum(sd => sd.FinalSalary);
        return departmentSalary;
    }

    public async Task<decimal> GetTotalSalaryAsync(int month, int year)
    {
        var salaryDetails = await salaryDetailRepository.GetSalaryDetailsAsync();
        var departmentSalary = salaryDetails
            .Where(sd => sd.Month == month && sd.Year == year)
            .Sum(sd => sd.FinalSalary);
        return departmentSalary;
    }

    public async Task<decimal> GetTotalAll()
    {
        var salaryDetails = await salaryDetailRepository.GetSalaryDetailsAsync();
        return salaryDetails.Sum(sd => sd.FinalSalary);
    }

    public async Task<decimal> GetEmployeeSalaryAsync(string employeeId, int month, int year)
    {
        var salaryDetail = await salaryDetailRepository.GetSalaryDetailAsync(employeeId, month, year);
        return salaryDetail?.FinalSalary ?? 0;
    }
}
