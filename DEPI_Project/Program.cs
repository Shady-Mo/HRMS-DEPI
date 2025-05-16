using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories.AttendanceRepo;
using DEPI_Project.Repositories;
using DEPI_Project.Repositories.CandidateRepo;
using DEPI_Project.Repositories.JobRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DEPI_Project.Repositories.ReportsRepo;
using Stripe;


namespace DEPI_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<CorpMgmt_SystemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<CorpMgmt_SystemContext>();

            builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
            builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
            builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IShiftScheduleRepository, ShiftScheduleRepository>(); 
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IPerformanceRepository,PerformanceRepository>();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Start}/{id?}"
            );
            // pattern: "{controller=DashboardAdmin}/{action=Index}/{id?}");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");


            app.Run();
        }
    }
}
