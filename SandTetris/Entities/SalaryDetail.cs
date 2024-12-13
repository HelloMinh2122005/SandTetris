namespace SandTetris.Entities;

public class SalaryDetail
{
    public string EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int Day { get; set; } = 1;
    public int Month { get; set; } = 1;
    public int Year { get; set; } = 1;
    public int BaseSalary { get; set; }
    public int Deposit { get; set; } = 0;

    public int DaysAbsent { get; set; }
    public int DaysOnLeave { get; set; }
    public int FinalSalary { get; set; }

    public bool IsDeposited => Deposit > 0;
    public string MonthYear => $"{Month}/{Year}";
    public int DaysWorking =>
        (DateTime.Now.Year > Year || (DateTime.Now.Year == Year && DateTime.Now.Month > Month))
        ? DateTime.DaysInMonth(Year, Month) - DaysAbsent - DaysOnLeave
        : DateTime.Now.Day - DaysAbsent - DaysOnLeave - Day + 1;
}
