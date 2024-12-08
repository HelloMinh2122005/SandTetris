using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SandTetris.Entities.Dto;

public partial class CheckInDto : ObservableObject
{
    [ObservableProperty]
    private int day;
    [ObservableProperty]
    private int month;
    [ObservableProperty]
    private int year;
    [ObservableProperty]
    private DateTime checkInTime = DateTime.MinValue;
    [ObservableProperty]
    private CheckInStatus status = CheckInStatus.Absent;
    [ObservableProperty]
    private string? note;
    [ObservableProperty]
    private Employee? employee;

    public static CheckInDto FromEntity(CheckIn entity)
    {
        return new CheckInDto
        {
            Day = entity.Day,
            Month = entity.Month,
            Year = entity.Year,
            CheckInTime = entity.CheckInTime,
            Status = entity.Status,
            Note = entity.Note,
            Employee = entity.Employee
        };
    }
}