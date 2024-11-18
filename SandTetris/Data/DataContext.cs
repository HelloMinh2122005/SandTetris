using Microsoft.EntityFrameworkCore;
using SandTetris.Entities;

namespace SandTetris.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<CheckIn> CheckIns { get; set; } = null!;
    public DbSet<SalaryDetail> SalaryDetails { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>()
                .HasOne(d => d.HeadOfDepartment)
                .WithMany()
                .HasForeignKey(d => d.HeadOfDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SalaryDetail>()
            .HasKey(s => new { s.EmployeeId, s.Month, s.Year });
    }
}
