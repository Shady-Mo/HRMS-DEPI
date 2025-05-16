using System.Net.NetworkInformation;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : Controller
    {
        CorpMgmt_SystemContext _context;
        public DashboardAdminController()
        {
            _context = new CorpMgmt_SystemContext();
        }
        public IActionResult Index()
        {
            ViewBag.EmployeeCount = _context.Employees.Count();
            ViewBag.DepartmentCount = _context.Departments.Count();
            ViewBag.OnLeaveCount = _context.Employees.Count(e => e.EmpStatus == StatusType.OnLeave);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            ViewBag.NewThisMonth = _context.Employees
                .Count(e => e.HireDate.Month == currentMonth && e.HireDate.Year == currentYear);

            var newEmployeesByMonth = _context.Employees
                .Where(e => e.HireDate.Year == DateTime.Now.Year)
                .GroupBy(e => e.HireDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Month) 
                .ToList();

          
            foreach (var item in newEmployeesByMonth)
            {
                Console.WriteLine($"Month: {item.Month}, Count: {item.Count}");
            }

          
            ViewBag.NewEmployeesMonthlyData = newEmployeesByMonth;

            var employeesByDept = _context.Departments
             .Select(d => new
             {
               DepartmentName = d.Name,
               EmployeeCount = d.Employees.Count()
             }).ToList();

            ViewBag.EmployeesByDepartment = employeesByDept;
            ViewBag.AttendanceList = _context.Attendances
                .Where(a => a.Date.Month == DateTime.Now.Month && a.Date.Year == DateTime.Now.Year)
                .Select(a => new
                {
                    EmployeeName = a.Employee.FirstName,
                    Date = a.Date,
                    Status = a.Status.ToString(),
                    CheckIn = a.TimeIn,
                    CheckOut = a.TimeOut
                }).ToList();


            return View();
        }
    }
}
