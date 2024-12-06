using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SandTetris.Entities;

public class Employee
{
    public string Id { get; set; }
    public required string FullName { get; set; }
    public DateTime DoB { get; set; }
    public required string Title { get; set; }

    //Avatar
    public byte[]? Avatar { get; set; } 
    public string? AvatarFileExtension { get; set; }

    public ICollection<SalaryDetail> SalaryDetails { get; set; } = [];
    public ICollection<CheckIn> CheckIns { get; set; } = [];


    //Deparment Navigation
    public string DepartmentId { get; set; }
    public Department Department { get; set; }
}
