
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Contexts
{
    public class AppDbContext: IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Owner -> Companies (One-to-Many)
            builder.Entity<Company>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCompanies)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Company -> Employees (One-to-Many)
            builder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId);

            // Employee -> User (One-to-One أو One-to-Many حسب اختيارك)
            builder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithMany() // أو .WithOne(u => u.Employee)
                .HasForeignKey(e => e.UserId);

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
    }
}
