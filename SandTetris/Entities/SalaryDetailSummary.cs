namespace SandTetris.Entities;

public class SalaryDetailSummary
{
    public string DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalSpent { get; set; }
    public string DateString => $"{Month}/{Year}";
}
