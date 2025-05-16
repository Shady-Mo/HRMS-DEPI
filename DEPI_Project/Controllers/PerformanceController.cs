using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.View_Model;
using DEPI_Project.Repositories;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Manager")]
    public class PerformanceController : Controller
    {
        private readonly IPerformanceRepository _performanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        public PerformanceController(IEmployeeRepository employeeRepository,
                                IProjectRepository projectRepository,
                                ITaskRepository taskRepository,
                                IPerformanceRepository performanceRepository)
        {
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _performanceRepository = performanceRepository;
            _taskRepository = taskRepository;

        }
        public ActionResult Index()
        {
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            var performances = _performanceRepository.GetAllPerformances();
            return View(performances);
        }

        #region Add
        public ActionResult Add()
        {
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            return View();
        }
        #endregion

        #region SaveAdd
        [HttpPost]
        public IActionResult SaveAdd(Performance performanceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", performanceViewModel);
            }
            var employee = _employeeRepository.GetById(performanceViewModel.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("Employee", "This Employee does not exist");
                return View("Add", performanceViewModel);
            }
            var task = _taskRepository.GetTaskById(performanceViewModel.TaskId);
            if (task == null)
            {
                ModelState.AddModelError("Task", "This Task does not exist");
                return View("Add", performanceViewModel);
            }

            var performance = new Performance
            {
                EmployeeId = employee.Id,
                TaskId = task.TaskId,
                Score = performanceViewModel.Score,
                Review = performanceViewModel.Review,
                Date = performanceViewModel.Date,
                Description = performanceViewModel.Description,
                ManagerId = performanceViewModel.ManagerId,

            };
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            _performanceRepository.AddPerformance(performance);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var performance = _performanceRepository.GetPerformanceById(id);
            if (performance == null)
            {
                return NotFound();
            }
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            return View(performance);
        }
        #endregion

        #region SaveEdit
        public IActionResult SaveEdit(Performance editPerformanceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdatePerformance", editPerformanceViewModel);
            }
            var performance = _performanceRepository.GetPerformanceById(editPerformanceViewModel.PerformanceId);
            if (performance == null)
            {
                ModelState.AddModelError("Performance", "This performance does not exist");
                return View("UpdatePerformance", editPerformanceViewModel);
            }
            performance.EmployeeId = editPerformanceViewModel.EmployeeId;
            performance.TaskId = editPerformanceViewModel.TaskId;
            performance.Score = editPerformanceViewModel.Score;
            performance.Review = editPerformanceViewModel.Review;
            performance.Date = editPerformanceViewModel.Date;
            performance.Description = editPerformanceViewModel.Description;
            performance.ManagerId = editPerformanceViewModel.ManagerId;
            _performanceRepository.UpdatePerformance(performance);
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region details
        public ActionResult Details(int id)
        {
            var performance = _performanceRepository.GetPerformanceById(id);
            if (performance == null)
            {
                return NotFound();
            }
            ViewData["TaskList"] = _taskRepository.GetListTasks();
            ViewData["EmployeeList"] = _employeeRepository.GetUsersAsSelectList();
            return View(performance);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {
            Performance performance = _performanceRepository.GetPerformanceById(id);
            if (ModelState.IsValid)
            {
                _performanceRepository.DeletePerformance(id);
                TempData["DeleteSuccess"] = "Employee deleted successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        #endregion

        #region ExportToExcel
        public IActionResult ExportToExcel()
        {
            var performances = _performanceRepository.GetAllPerformances()
                .Select(p => new
                {
                    p.Review,
                    Date = p.Date.ToString("MM/dd/yyyy"),
                    p.Description,
                    EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                    TaskName = p.Task.Title,
                    ManagerName = p.Manager.FirstName + " " + p.Manager.LastName
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Performance Reviews");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Review";
                worksheet.Cell(currentRow, 2).Value = "Date";
                worksheet.Cell(currentRow, 3).Value = "Description";
                worksheet.Cell(currentRow, 4).Value = "Employee";
                worksheet.Cell(currentRow, 5).Value = "Task";
                worksheet.Cell(currentRow, 6).Value = "Manager";

                // Data
                foreach (var p in performances)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = p.Review;
                    worksheet.Cell(currentRow, 2).Value = p.Date;
                    worksheet.Cell(currentRow, 3).Value = p.Description;
                    worksheet.Cell(currentRow, 4).Value = p.EmployeeName;
                    worksheet.Cell(currentRow, 5).Value = p.TaskName;
                    worksheet.Cell(currentRow, 6).Value = p.ManagerName;
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Performance-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }

        #endregion

        #region Charts

        //[HttpGet]
        //public async Task<JsonResult> GetScoreDistributionData()
        //{
        //    var data = await _performanceRepository.GetScoreDistributionDataAsync();
        //    return Json(data);
        //}


        //[HttpGet]
        //public async Task<JsonResult> GetEmployeePerformanceOverTime(string employeeId)
        //{
        //    if (string.IsNullOrEmpty(employeeId))
        //    {
        //        return Json(new { error = "Employee ID is required" });
        //    }

        //    var data = await _performanceRepository.GetEmployeePerformanceOverTimeAsync(employeeId);
        //    return Json(data);
        //}
        #endregion


    }
}


