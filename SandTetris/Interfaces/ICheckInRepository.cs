using SandTetris.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Interfaces;

public interface ICheckInRepository
{
    //Get 1 check-in
    Task<CheckIn?> GetCheckInByIdAsync(string id);
    //Get all check-ins
    Task<IEnumerable<CheckIn>> GetCheckInsAsync();
    //Add check-in
    Task AddCheckInAsync(CheckIn checkIn);
    //Update check-in
    Task UpdateCheckInAsync(CheckIn checkIn);
    //Delete check-in
    Task DeleteCheckInAsync(CheckIn checkIn);
}
