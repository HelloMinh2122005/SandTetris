namespace SandTetris.Entities;

public class CheckInSummary
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalWorking { get; set; }
    public int TotalOnLeave { get; set; }
    public int TotalAbsent { get; set; }
    public string DateString => $"{Day}/{Month}/{Year}";
}