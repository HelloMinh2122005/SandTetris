namespace SandTetris.Entities;

public class CheckIn
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Day { get; set; }
    public CheckInStatus Status { get; set; }
    public string? Note { get; set; }

    //Employee Navigation
    public string EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}

public enum CheckInStatus
{
    Absent,
    Working,
    OnLeave
}
