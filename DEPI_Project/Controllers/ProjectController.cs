using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using DEPI_Project.Models.View_Model;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Identity;

namespace DEPI_Project.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectRepository projectRepository;
        private readonly IManagerRepository managerRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly CorpMgmt_SystemContext context;


        public ProjectController(IProjectRepository _projectRepository,
            IDepartmentRepository _departmentRepository,
            IManagerRepository _managerRepository,
            IEmployeeRepository _employeeRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            CorpMgmt_SystemContext _context)
        {
            projectRepository = _projectRepository;
            departmentRepository = _departmentRepository;
            managerRepository = _managerRepository;
            employeeRepository = _employeeRepository;
            context = _context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        #region Index
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
            ViewData["ManagerList"] = employeeRepository.GetManagers()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.UserName}"
                }).ToList();
            ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();
            List<Project> projects = projectRepository.GetAllProjects();
            return View("Index" , projects);
        }
        #endregion

        #region Project 
        //public IActionResult ShowManagerProjects(string id)
        //{
        //    return View("ShowManagerProjects", projectRepository.GetProjectsByManagerId(id));
        //}
        #endregion


        #region Add
        [Authorize(Roles = "Admin")]
        public IActionResult Add(ProjectViewModel project)
        {
            // Load required data for dropdowns
            ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
            ViewData["ManagerList"] = employeeRepository.GetManagers()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.UserName}"
                }).ToList();
            ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();

            return View("Add", project);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveAdd(ProjectViewModel DataFromReq)
        {
            if (DataFromReq.DueDate < DataFromReq.StartDate) {
                ModelState.AddModelError("Project.DueDate", "Due Date must be after Start Date");
                ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
                ViewData["ManagerList"] = employeeRepository.GetManagers()
                    .Select(e => new SelectListItem {
                        Value = e.Id.ToString(),
                        Text = $"{e.UserName}"
                    }).ToList();
                ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();

                return View("Add", DataFromReq);
            }

            if (ModelState.IsValid)
            {
                // Add project
                Project project = new Project {
                    Name = DataFromReq.Name,
                    Description = DataFromReq.Description,
                    StartDate = DataFromReq.StartDate,
                    Status = DataFromReq.Status,
                    Budget = DataFromReq.Budget,
                    Priority = DataFromReq.Priority,
                    DueDate = DataFromReq.DueDate,
                    DepartmentId = DataFromReq.DepartmentId,
                    ManagerId = DataFromReq.ManagerId
                };

                if (DataFromReq.ProjectEmployees.Count == 0) {
                    ModelState.AddModelError("ProjectEmployees", "You have to choose at least 1 employee");
                    ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
                    ViewData["ManagerList"] = employeeRepository.GetManagers()
                        .Select(e => new SelectListItem {
                            Value = e.Id.ToString(),
                            Text = $"{e.UserName}"
                        }).ToList();
                    ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();

                    return View("Add", DataFromReq);
                }

                context.Projects.Add(project);
                context.SaveChanges();

                // Assign employees to project
                foreach (var employeeId in DataFromReq.ProjectEmployees)
                {

                    if (employeeId != null)
                    {
                        var projectEmployee = new AssignedEmployees
                        {
                            ProjectId = project.ProjectId,
                            EmployeeId = employeeId,
                        };
                        projectRepository.AddAssignedEmployee(projectEmployee);
                    }
                }

                TempData["Success"] = "Project saved successfully with assigned employees";
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
            ViewData["ManagerList"] = employeeRepository.GetManagers()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.UserName}"
                }).ToList();
            ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();

            return View("Add", DataFromReq);
        }
        #endregion




        #region Edit
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
            ViewData["ManagerList"] = employeeRepository.GetManagers()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.UserName}"
                }).ToList();
            ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();

            Project project = projectRepository.GetProjectById(id);

            List<string> employeesId = context.AssignedEmployees
                .Where(e => e.ProjectId == id)
                .Select(e => e.EmployeeId)
                .ToList();

            ProjectViewModel projectViewModel = new ProjectViewModel {
                DepartmentId = project.DepartmentId,
                Budget = project.Budget,
                Description = project.Description,
                DueDate = project.DueDate,
                ManagerId = project.ManagerId,
                Name = project.Name,
                Priority = project.Priority,
                StartDate = project.StartDate,
                Status = project.Status,
                ProjectEmployees = employeesId
            };
            if (project == null)
                return NotFound();
            ViewBag.ProjectId = id;
            return View("Edit", projectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveEdit(ProjectViewModel DataFromReq, int id) {
            if (DataFromReq.DueDate < DataFromReq.StartDate) {
                ModelState.AddModelError("Project.DueDate", "Due Date must be after Start Date");
                ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
                ViewData["ManagerList"] = employeeRepository.GetManagers()
                    .Select(e => new SelectListItem {
                        Value = e.Id.ToString(),
                        Text = $"{e.UserName}"
                    }).ToList();
                ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();
                ViewBag.ProjectId = id;
                return View("Edit", DataFromReq);
            }

            if (ModelState.IsValid) {
                Project ProjectFromDB = projectRepository.GetProjectById(id);
                if (ProjectFromDB == null) {
                    ViewBag.ProjectId = id;
                    return NotFound();
                }

                ProjectFromDB.Name = DataFromReq.Name;
                ProjectFromDB.Description = DataFromReq.Description;
                ProjectFromDB.StartDate = DataFromReq.StartDate;
                ProjectFromDB.Status = DataFromReq.Status;
                ProjectFromDB.Budget = DataFromReq.Budget;
                ProjectFromDB.Priority = DataFromReq.Priority;
                ProjectFromDB.DueDate = DataFromReq.DueDate;
                ProjectFromDB.DepartmentId = DataFromReq.DepartmentId;
                ProjectFromDB.ManagerId = DataFromReq.ManagerId;

                if (DataFromReq.ProjectEmployees.Count == 0) {
                    ModelState.AddModelError("ProjectEmployees", "You have to choose at least 1 employee");
                    ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
                    ViewData["ManagerList"] = employeeRepository.GetManagers()
                        .Select(e => new SelectListItem {
                            Value = e.Id.ToString(),
                            Text = $"{e.UserName}"
                        }).ToList();
                    ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();
                    ViewBag.ProjectId = id;
                    return View("Edit", DataFromReq);
                }

                projectRepository.UpdateProject(ProjectFromDB);

                var assignedProject = context.AssignedEmployees.Where(ap => ap.ProjectId == id).ToList();
                context.AssignedEmployees.RemoveRange(assignedProject);

                foreach (var employeeId in DataFromReq.ProjectEmployees) {

                    if (employeeId != null) {
                        var projectEmployee = new AssignedEmployees {
                            ProjectId = ProjectFromDB.ProjectId,
                            EmployeeId = employeeId,
                        };
                        projectRepository.AddAssignedEmployee(projectEmployee);
                    }
                }

                TempData["Success"] = "Project updated successfully with assigned employees";
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentList"] = departmentRepository.GetDepartmentsAsSelectList();
            ViewData["ManagerList"] = employeeRepository.GetManagers()
                .Select(e => new SelectListItem {
                    Value = e.Id.ToString(),
                    Text = $"{e.UserName}"
                }).ToList();
            ViewData["EmpList"] = employeeRepository.GetUsersAsSelectList();
            ViewBag.ProjectId = id;

            return View("Edit", DataFromReq);
        }

        #endregion

        #region Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var project = projectRepository.GetProjectById(id);
            return View("Delete", project);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveDelete(int id)
        {
            var project = projectRepository.GetProjectById(id);
            projectRepository.DeleteProject(project);
            var assignedProject = context.AssignedEmployees.Where(ap => ap.ProjectId == id).ToList();
            context.AssignedEmployees.RemoveRange(assignedProject);
            return RedirectToAction(nameof(Index));
        }
        #endregion
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetProjects() {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            var projects = projectRepository.GetProjectsByManager(userId);

            return View(projects);
        }
    }
}