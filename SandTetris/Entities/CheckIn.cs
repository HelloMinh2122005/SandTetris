using System.ComponentModel.DataAnnotations;

namespace SandTetris.Entities;

public class CheckIn
{
    [Key]
    public int Day { get; set; }
    [Key]
    public int Month { get; set; }
    [Key]
    public int Year { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.MinValue;
    
    public CheckInStatus Status { get; set; } = CheckInStatus.Absent;
    public string? Note { get; set; }

    //Employee Navigation
    [Key]
    public string EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}

public enum CheckInStatus
{
    Absent,
    Working,
    OnLeave
}
