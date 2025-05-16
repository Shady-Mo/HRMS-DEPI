using System.Security.Claims;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.View_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Project.Controllers
{
    public class DashboardEmployeeController : Controller
    {
        private readonly CorpMgmt_SystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public DashboardEmployeeController(CorpMgmt_SystemContext context , UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public ActionResult Index()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var userId = _userManager.Users
                .FirstOrDefault(u => u.UserName == userName)?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            var viewModel = new EmployeeDashboardViewModel
            {
                TotalLeave = _context.Leaves.Count(l => l.EmpId == userId),
                TotalHoliday = _context.Holidays.Count(),
                TotalTask = _context.ProjectTasks.Count(t => t.ManagerId== userId || t.EmployeeId==userId),
                TotalProject = _context.Projects.Count(p => p.AssignedEmployees.Any(e => e.EmployeeId== userId) || p.ManagerId==userId)
            };

            return View(viewModel);

        }



    }
}
