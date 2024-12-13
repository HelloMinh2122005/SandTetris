﻿using SandTetris.Data;
using SandTetris.Entities;

namespace SandTetris.Interfaces;

public interface ICheckInRepository
{
    //Get 1 check-in
    Task<CheckIn?> GetCheckInByIdAsync(string employeeId, int day, int month, int year);
    //Get all check-ins
    Task<IEnumerable<CheckIn>> GetCheckInsAsync();
    //Add check-in
    Task AddCheckInAsync(CheckIn checkIn);
    //Update check-in
    Task UpdateCheckInAsync(CheckIn checkIn);
    //Delete check-in
    Task DeleteCheckInAsync(CheckIn checkIn);
    Task<bool> CheckValidDayAsync(string departmentId, int day, int month, int year);
    Task AddCheckInsForDepartmentAsync(string departmentId, int day, int month, int year);
    Task DeleteCheckInForDepartmentAsync(string departmentId, int day, int month, int year);
    Task<IEnumerable<CheckIn>> GetCheckInsForDepartmentAsync(string departmentId, int day, int month, int year);
    Task<IEnumerable<CheckIn>> GetCheckInsForEmployeeAsync(string employeeId, int month, int year);
    Task<IEnumerable<CheckInSummary>> GetCheckInSummariesAsync(string departmentId, int month, int year);
    Task<IEnumerable<CheckInSummary>> GetAllCheckInSummariesAsync(string departmentId);
    Task UpdateEmployeeCheckInAsync(string employeeId, int day, int month, int year, CheckInStatus status, DateTime checkInTime, string note);
}
