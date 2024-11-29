namespace SandTetris.Entities;

public class SalaryDetailSummary
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalSpent { get; set; }
    public string DateString => $"{Month}/{Year}";
}
