
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
            builder.Entity<Company>().ToTable("Company");
            builder.Entity<Nationality>().ToTable("Nationality");
            builder.Entity<Company>().ToTable("Company");
            builder.Entity<Nationality>().ToTable("Nationality");
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

            builder.Entity<EmploymentAssignment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.EmploymentAssignments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentAssignment>()
                .HasOne(a => a.Manager)
                .WithMany()
                .HasForeignKey(a => a.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentAssignment>()
                .HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentAssignment>()
                .HasOne(a => a.Position)
                .WithMany()
                .HasForeignKey(a => a.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkingSchedule>()
                .HasOne(s => s.Company)
                .WithMany(c => c.WorkingSchedules)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkingSchedule>()
                .HasIndex(s => new { s.CompanyId, s.Name })
                .IsUnique();

            builder.Entity<WorkingScheduleDay>()
                .HasOne(d => d.WorkingSchedule)
                .WithMany(s => s.Days)
                .HasForeignKey(d => d.WorkingScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkingScheduleDay>()
                .HasIndex(d => new { d.WorkingScheduleId, d.DayOfWeek })
                .IsUnique();

            builder.Entity<EmploymentAssignment>()
                .HasOne(a => a.WorkingSchedule)
                .WithMany()
                .HasForeignKey(a => a.WorkingScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Position>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Positions)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentAssignment>()
                .HasIndex(a => new { a.EmployeeId, a.EffectiveFrom })
                .IsUnique();

            builder.Entity<SystemPage>().HasIndex(x => x.Key).IsUnique();
            builder.Entity<PermissionDefinition>().HasIndex(x => x.Key).IsUnique();
            builder.Entity<PermissionDefinition>()
                .HasOne(x => x.SystemPage).WithMany(x => x.Permissions)
                .HasForeignKey(x => x.SystemPageId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasOne(x => x.Company).WithMany()
                .HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CompanyRole>().HasIndex(x => new { x.CompanyId, x.NormalizedName }).IsUnique();
            builder.Entity<CompanyRole>().HasOne(x => x.Company).WithMany()
                .HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CompanyRolePermission>().HasIndex(x => new { x.CompanyRoleId, x.PermissionDefinitionId }).IsUnique();
            builder.Entity<CompanyRolePermission>().HasOne(x => x.CompanyRole).WithMany(x => x.Permissions)
                .HasForeignKey(x => x.CompanyRoleId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CompanyRolePermission>().HasOne(x => x.PermissionDefinition).WithMany()
                .HasForeignKey(x => x.PermissionDefinitionId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CompanyUserRole>().HasIndex(x => new { x.UserId, x.CompanyRoleId }).IsUnique();
            builder.Entity<CompanyUserRole>().HasOne(x => x.User).WithMany(x => x.CompanyRoles)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CompanyUserRole>().HasOne(x => x.CompanyRole).WithMany(x => x.Users)
                .HasForeignKey(x => x.CompanyRoleId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<SystemRole>().HasIndex(x => x.NormalizedName).IsUnique();
            builder.Entity<SystemRolePermission>().HasIndex(x => new { x.SystemRoleId, x.PermissionDefinitionId }).IsUnique();
            builder.Entity<SystemRolePermission>().HasOne(x => x.SystemRole).WithMany(x => x.Permissions)
                .HasForeignKey(x => x.SystemRoleId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SystemRolePermission>().HasOne(x => x.PermissionDefinition).WithMany()
                .HasForeignKey(x => x.PermissionDefinitionId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SystemUserRole>().HasIndex(x => new { x.UserId, x.SystemRoleId }).IsUnique();
            builder.Entity<SystemUserRole>().HasOne(x => x.User).WithMany(x => x.SystemRoles)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SystemUserRole>().HasOne(x => x.SystemRole).WithMany(x => x.Users)
                .HasForeignKey(x => x.SystemRoleId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserPermissionOverride>().HasIndex(x => new { x.UserId, x.PermissionDefinitionId }).IsUnique();
            builder.Entity<UserPermissionOverride>().HasOne(x => x.User).WithMany(x => x.PermissionOverrides)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserPermissionOverride>().HasOne(x => x.PermissionDefinition).WithMany()
                .HasForeignKey(x => x.PermissionDefinitionId).OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<EmploymentAssignment> EmploymentAssignments { get; set; }
        public DbSet<WorkingSchedule> WorkingSchedules { get; set; }
        public DbSet<WorkingScheduleDay> WorkingScheduleDays { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<SystemPage> SystemPages { get; set; }
        public DbSet<PermissionDefinition> PermissionDefinitions { get; set; }
        public DbSet<CompanyRole> CompanyRoles { get; set; }
        public DbSet<CompanyRolePermission> CompanyRolePermissions { get; set; }
        public DbSet<CompanyUserRole> CompanyUserRoles { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<SystemRolePermission> SystemRolePermissions { get; set; }
        public DbSet<SystemUserRole> SystemUserRoles { get; set; }
        public DbSet<UserPermissionOverride> UserPermissionOverrides { get; set; }
    }
}
