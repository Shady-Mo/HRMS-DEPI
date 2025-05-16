using DEPI_Project.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Models.CorpMgmt_System.Context
{
    public class CorpMgmt_SystemContext : IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=HRMS;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
            }

        }
        public CorpMgmt_SystemContext(DbContextOptions<CorpMgmt_SystemContext> option) : base(option)
        {

        }

        public CorpMgmt_SystemContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new LeaveEntityConfiguration()
                .Configure(modelBuilder.Entity<Leave>());

            new DepartmentEntityConfiguration()
                .Configure(modelBuilder.Entity<Department>());

            new AttendanceEntityConfiguration()
                .Configure(modelBuilder.Entity<Attendance>());

            new ShiftScheduleEntityConfiguration()
              .Configure(modelBuilder.Entity<ShiftSchedule>());

            new PaymentEntityConfiguration()
                .Configure(modelBuilder.Entity<Payment>());

            new HolidayEntityConfiguration()
                .Configure(modelBuilder.Entity<Holiday>());

            new TaskEntityConfiguration()
           .Configure(modelBuilder.Entity<ProjectTask>());

            new ProjectEntityConfiguration()
            .Configure(modelBuilder.Entity<Project>());

            new PerformanceEntityConfguration()
            .Configure(modelBuilder.Entity<Performance>());
            
            modelBuilder.ApplyConfiguration(new AppUserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new JobEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CandidateEntityConfiguration());

            modelBuilder.Entity<AssignedEmployees>()
                .HasKey(pe => new { pe.ProjectId, pe.EmployeeId });

            modelBuilder.Entity<AssignedEmployees>()
                .HasOne(pe => pe.Project)
                .WithMany(p => p.AssignedEmployees)
                .HasForeignKey(pe => pe.ProjectId);

            modelBuilder.Entity<AssignedEmployees>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.AssignedEmployees)
                .HasForeignKey(pe => pe.EmployeeId);

            base.OnModelCreating(modelBuilder);


        }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ShiftSchedule> ShiftSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<AssignedEmployees> AssignedEmployees { get; set; }



    }
}
