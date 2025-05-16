using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.View_Model;
using DEPI_Project.Repositories;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;
using TaskStatus = DEPI_Project.Models.CorpMgmt_System.TaskStatus;

namespace DEPI_Project.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskController(IEmployeeRepository employeeRepository,
                                IProjectRepository projectRepository,
                                ITaskRepository taskRepository,
                                IHttpContextAccessor httpContextAccessor,
                                UserManager<AppUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            var project = _taskRepository.GetTasksByManagerId(userId);
            if (project == null) {
                ModelState.AddModelError("Task", "This task does not exist");
                return View("Index", project);
            }
            
            return View("Index", project);
        }
        public IActionResult EmpTask()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            var project = _taskRepository.GetTasks(userId);
            if (project == null)
            {
                ModelState.AddModelError("Task", "This task does not exist");
                return View("EmpTask", project);
            }

            return View("EmpTask", project);
        }


        public IActionResult Add()
        {
            ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(TaskViewModel taskViewModel)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            if (!ModelState.IsValid)
            {
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Add", taskViewModel);
            }
            var employee = _employeeRepository.GetById(taskViewModel.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("Employee", "This employee does not exist");
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Add", taskViewModel);
            }
            if (!Enum.TryParse<TaskStatus>(taskViewModel.Status.ToString(), true, out var status))
            {
                ModelState.AddModelError("Status", "This status is not correct");
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Add", taskViewModel);
            }
            if (!Enum.TryParse<PriorityLevel>(taskViewModel.Priority.ToString(), true, out var priority))
            {
                ModelState.AddModelError("Priority", "This priority is not correct");
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Add", taskViewModel);
            }
            var project = _projectRepository.GetProjectById(taskViewModel.ProjectId);
            var employeeInProject = project.AssignedEmployees.FirstOrDefault(e => e.EmployeeId == employee.Id);
            if (employeeInProject == null)
            {
                ModelState.AddModelError("Task", "This project is not assigned to the employee");
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Add", taskViewModel);
            }

            if (employee.AssignedTasks == null)
            {
                employee.AssignedTasks = new List<ProjectTask>();
            }
            var task = new ProjectTask
            {
                ProjectId = taskViewModel.ProjectId,
                EmployeeId = employee.Id,
                Status = status,
                ManagerId = userId,
                Description = taskViewModel.Description,
                Title = taskViewModel.Title,
                DueDate = taskViewModel.DueDate,
                StartDate = taskViewModel.StartDate,
                CreatedDate = taskViewModel.CreatedDate,
                Priority = priority,
                EstimatedHours = taskViewModel.EstimatedHours,
                ActualHours = taskViewModel.ActualHours,
                CompletedDate = taskViewModel.CompletedDate
            };
            _taskRepository.AddTask(task);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
            {
                ModelState.AddModelError("Task", "This task does not exist");
                return View("Index", task);
            }
            ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
            ViewBag.TaskId = id;
            TaskViewModel taskViewModel = new TaskViewModel {
                Description = task.Description,
                ProjectId = task.ProjectId,
                ActualHours = task.ActualHours,
                CompletedDate = task.CompletedDate,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                EmployeeId = task.EmployeeId,
                EstimatedHours = task.EstimatedHours,
                Priority = task.Priority,
                StartDate = task.StartDate,
                Status = (Models.View_Model.TaskStatus)task.Status,

                Title = task.Title
            };
            return View("Edit", taskViewModel);
        }

        [HttpPost]
        public IActionResult SaveEdit(TaskViewModel taskViewModel, int id)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            if (!ModelState.IsValid)
            {
                ViewBag.TaskId = id;
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                return View("Edit", taskViewModel);
            }
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
            {
                ModelState.AddModelError("Task", "This task does not exist");
                return View("Index", task);
            }
            var employee = _employeeRepository.GetById(taskViewModel.EmployeeId);
            if (employee == null)
            {
                ViewBag.TaskId = id;
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                ModelState.AddModelError("Employee", "This employee does not exist");
                return View("Edit", taskViewModel);
            }
            if (!Enum.TryParse<TaskStatus>(taskViewModel.Status.ToString(), true, out var status))
            {
                ViewBag.TaskId = id;
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                ModelState.AddModelError("Status", "This status is not correct");
                return View("Edit", taskViewModel);
            }
            if (!Enum.TryParse<PriorityLevel>(taskViewModel.Priority.ToString(), true, out var priority))
            {
                ViewBag.TaskId = id;
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                ModelState.AddModelError("Priority", "This priority is not correct");
                return View("Edit", taskViewModel);
            }
            var project = _projectRepository.GetProjectById(taskViewModel.ProjectId);
            var employeeInProject = project.AssignedEmployees.FirstOrDefault(e => e.EmployeeId == employee.Id);
            if (employeeInProject == null)
            {
                ViewBag.TaskId = id;
                ViewBag.EmployeeList = _employeeRepository.GetEmployeesAsSelectList();
                ViewBag.ProjectList = _projectRepository.GetProjectsByManagerId(userId);
                ModelState.AddModelError("Task", "This project is not assigned to the employee");
                return View("Edit", taskViewModel);
            }
            task.ProjectId = taskViewModel.ProjectId;
            task.EmployeeId = employee.Id;
            
            task.ManagerId = project.ManagerId;
            task.Description = taskViewModel.Description;
            task.Title = taskViewModel.Title;
            task.DueDate = taskViewModel.DueDate;
            task.StartDate = taskViewModel.StartDate;
            task.CreatedDate = taskViewModel.CreatedDate;
            task.Priority = priority;
            task.EstimatedHours = taskViewModel.EstimatedHours;
            task.ActualHours = taskViewModel.ActualHours;
            task.CompletedDate = taskViewModel.CompletedDate;
            task.Status =status;
            _taskRepository.UpdateTask(task);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            ProjectTask task = _taskRepository.GetTaskById(id);
            if (task == null)
            {
                ModelState.AddModelError("Task", "This task does not exist");
                return View("Index", task);
            }
            _taskRepository.DeleteTask(id);
            return RedirectToAction("Index");
        }
    }
}
