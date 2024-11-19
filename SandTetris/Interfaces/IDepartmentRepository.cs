using SandTetris.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Interfaces;

public interface IDepartmentRepository
{
    //Get 1 department
    Task<Department?> GetDepartmentByIdAsync(string id);
    //Get all departments
    Task<IEnumerable<Department>> GetDepartmentsAsync();
    //Add department
    Task AddDepartmentAsync(Department department);
    //Update department
    Task UpdateDepartmentAsync(Department department);
    //Delete department
    Task DeleteDepartmentAsync(Department department);
    Task UpdateDeparmentHeadAsync(string departmentId, string employeeId);
}
