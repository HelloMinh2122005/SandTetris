namespace SandTetris.Entities;

public class SalaryDetail
{
    public string EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int Month { get; set; }
    public int Year { get; set; }
    public int MinWorkingDays { get; set; }
    public int AllowedOnLeaveDays { get; set; }
    public decimal BaseSalary { get; set; }
}
