using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{

}
