using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Entities;

public class Employee
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string FullName { get; set; }
    public DateTime DoB { get; set; }
    public required string Title { get; set; }

}
