using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly UserManager<AppUser> context;
        private readonly CorpMgmt_SystemContext dbcontext;

        public EmployeeRepository(UserManager<AppUser> context, CorpMgmt_SystemContext dbcontext)
        {
            this.context = context;
            this.dbcontext = dbcontext;
        }

        public async Task<IdentityResult> AddPassword(Employee employee, string password)
        {
            var result = await context.CreateAsync(employee, password);
            await context.AddToRoleAsync(employee, "Employee");
            return result;
        }

        public async Task<string> CheckEmployeeLogin(string email, string password)
        {
            if (email == null || password == null)
            {
                return "Invalid email or password";
            }
            var employee = await context.FindByEmailAsync(email);
            if (employee == null)
            {
                return "Invalid email or password";
            }
            var isEmployee = await context.IsInRoleAsync(employee, "Employee");
            if (!isEmployee)
            {
                return "User is not an employee";
            }
            var result = await context.CheckPasswordAsync(employee, password);
            if (result)
            {
                return null;
            }
            return "Invalid email or password";
        }

        public bool CheckEmployeeByEmail(string email)
        {
            return context.Users.Any(e => e.Email == email);
        }

        public bool CheckEmployeeByUserName(string userName)
        {
            return context.Users.Any(e => e.UserName == userName);
        }

        public List<Employee> GetAll()
        {
            return dbcontext.Users
                .OfType<Employee>()
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .ToList();
        }

        public List<Employee> GetManagers()
        {
            return dbcontext.Users
                .OfType<Employee>()
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .Where(e => e.Type=="Manager")
                .ToList();
        }
        public List<Employee> GetAdmins()
        {
            return dbcontext.Users
                .OfType<Employee>()
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .Where(e => e.Type == "Admin")
                .ToList();
        }
        public List<SelectListItem> GetAdminList()
        {
            return context.Users
                .OfType<Employee>()
                .Where(e => e.Type == "Admin")
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.UserName
                })
                .ToList();
        }
        public Employee GetById(string id)
        {
            return dbcontext.Users
                .OfType<Employee>()
                .Include(e => e.Department)
                .Include(e => e.Projects)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Employee employee)
        {
            dbcontext.Add(employee);
        }

        public void Update(Employee employee)
        {
            dbcontext.Update(employee);
        }

        public void Delete(Employee employee)
        {
            dbcontext.Remove(employee);
        }

        public void SaveInDB()
        {
            dbcontext.SaveChanges();
        }

        public List<SelectListItem> GetUsersAsSelectList()
        {
            return context.Users
                .Where(u => u.Type == "Employee")
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.UserName
                })
                .ToList();
        }
        //************* Emp dashboard *************
        #region Attendance

        public List<Attendance> GetMyAttendances(string userId)
        {
            return dbcontext.Attendances
                .Where(a => a.Employee.Id == userId)
                .OrderByDescending(a => a.Date)
                .ToList();
        }

        public Attendance? GetTodayAttendance(string userId)
        {
            return dbcontext.Attendances
                .FirstOrDefault(a => a.AppUserId == userId && a.Date.Date == DateTime.Today);
        }

        public void AddAttendance(Attendance attendance)
        {
            dbcontext.Attendances.Add(attendance);
            dbcontext.SaveChanges();
        }

        public void UpdateAttendance(Attendance attendance)
        {
            dbcontext.Attendances.Update(attendance);
            dbcontext.SaveChanges();
        }



        #endregion

        #region Profile
        public Employee? GetEmployeeProfile(string userId)
        {
            return dbcontext.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .FirstOrDefault(e => e.Id == userId);
        }
        #endregion


        #region Leave
        public List<Leave> GetMyLeaves(string userId)
        {
            return dbcontext.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Employee.Id == userId)
                .OrderByDescending(l => l.StartDate)
                .ToList();
        }

        public void SubmitLeave(Leave leave)
        {
            dbcontext.Leaves.Add(leave);
            dbcontext.SaveChanges();
        }

        public void CancelLeave(int leaveId, string userId)
        {
            var leave = dbcontext.Leaves
                .FirstOrDefault(l => l.LeaveId == leaveId && l.Employee.Id == userId && l.Status == LeaveStatus.Pending);
            if (leave != null)
            {
                dbcontext.Leaves.Remove(leave);
                dbcontext.SaveChanges();
            }
        }

        public List<SelectListItem> GetEmployeesAsSelectList()
        {
            return context.Users
                .Where(u => u.Type == "Employee")
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.UserName
                })
                .ToList();
        }


        #endregion

        
    }
}