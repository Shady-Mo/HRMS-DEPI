using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.View_Model;
using DEPI_Project.Repositories;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Project.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IPerformanceRepository _performanceRepository;

        public ManagerController(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IPerformanceRepository performanceRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _performanceRepository = performanceRepository;
        }

        #region Index
        public IActionResult Index()
        {
            //ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            var managers = _employeeRepository.GetManagers();
            return View("Index", managers);
        }
        #endregion

        #region Add
        //public IActionResult Add()
        //{
        //    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //    return View("Add");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SaveAdd(Employee employee)
        //{
        //    employee.Type = "Manager";
        //    ModelState.Remove("Type");

        //    if (ModelState.IsValid)
        //    {
        //        if (_employeeRepository.CheckEmployeeByEmail(employee.Email))
        //        {
        //            ModelState.AddModelError("Email", "Email already exists");
        //            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //            return View("Add", employee);
        //        }

        //        _employeeRepository.Add(employee);
        //        _employeeRepository.SaveInDB();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //    return View("Add", employee);
        //}
        #endregion

        #region Edit
        //public IActionResult Edit(string id)
        //{
        //    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //    var employee = _employeeRepository.GetById(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    return View("Edit", employee);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SaveEdit(Employee employee)
        //{
        //    employee.Type = "Manager";
        //    ModelState.Remove("Type");

        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //        return View("Edit", employee);
        //    }

        //    var employeeFromDB = _employeeRepository.GetById(employee.Id);
        //    if (employeeFromDB == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update new fields
        //    employeeFromDB.Skills = employee.Skills;
        //    employeeFromDB.Address = employee.Address;
        //    employeeFromDB.Nationality = employee.Nationality;

        //    // Rest of existing updates
        //    employeeFromDB.FirstName = employee.FirstName;
        //    employeeFromDB.LastName = employee.LastName;
        //    employeeFromDB.Email = employee.Email;
        //    employeeFromDB.PhoneNumber = employee.PhoneNumber;
        //    employeeFromDB.Position = employee.Position;
        //    employeeFromDB.DeptId = employee.DeptId;

        //    try
        //    {
        //        _employeeRepository.Update(employeeFromDB);
        //        _employeeRepository.SaveInDB();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error saving changes: " + ex.Message);
        //        ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
        //        return View("Edit", employee);
        //    }
        //}
        #endregion

        #region Delete
        public IActionResult Delete(string id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View("Delete", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveDelete(string id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }

            _employeeRepository.Delete(employee);
            _employeeRepository.SaveInDB();
            return RedirectToAction(nameof(Index));
        }
        #endregion

         #region Export to Excel
            public IActionResult ExportManagersToExcel()
        {
            var managers = _employeeRepository.GetManagers();
            if (managers == null || !managers.Any())
            {
                return NotFound("No managers found to export.");
            }

            var managersData = managers
                .Select(m => new
                {
                    FullName = m.FirstName + " " + m.LastName,
                    Email = m.Email ?? "N/A",
                    PhoneNumber = m.PhoneNumber ?? "N/A",
                    Position = m.Position ?? "N/A",
                    Department = m.Department?.Name ?? "N/A",
                    ProjectName = m.Projects != null && m.Projects.Any() ? m.Projects.First().Name : "N/A"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Managers");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Full Name";
                worksheet.Cell(currentRow, 2).Value = "Email";
                worksheet.Cell(currentRow, 3).Value = "Phone";
                worksheet.Cell(currentRow, 4).Value = "Position";
                worksheet.Cell(currentRow, 5).Value = "Department";
                worksheet.Cell(currentRow, 6).Value = "Project Name";

                // Data
                foreach (var m in managersData)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = m.FullName;
                    worksheet.Cell(currentRow, 2).Value = m.Email;
                    worksheet.Cell(currentRow, 3).Value = m.PhoneNumber;
                    worksheet.Cell(currentRow, 4).Value = m.Position;
                    worksheet.Cell(currentRow, 5).Value = m.Department;
                    worksheet.Cell(currentRow, 6).Value = m.ProjectName;
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
                        $"Managers-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion

        #region Export to PDF
        public IActionResult ExportManagersToPdf()
        {
            var managers = _employeeRepository.GetManagers();
            if (managers == null || !managers.Any())
            {
                return NotFound("No managers found to export.");
            }

            var managersData = managers
                .Select(m => new
                {
                    FullName = m.FirstName + " " + m.LastName,
                    Email = m.Email ?? "N/A",
                    PhoneNumber = m.PhoneNumber ?? "N/A",
                    Position = m.Position ?? "N/A",
                    Department = m.Department?.Name ?? "N/A",
                    ProjectName = m.Projects != null && m.Projects.Any() ? m.Projects.First().Name : "N/A"
                })
                .ToList();

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    Document document = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Add title
                    document.Add(new Paragraph("Managers Report - " + DateTime.Now.ToString("yyyy-MM-dd"), new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                    document.Add(new Paragraph(" "));

                    // Add table
                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100;
                    table.AddCell(new PdfPCell(new Phrase("Full Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Email", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Phone", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Position", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Department", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Project Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    foreach (var m in managersData)
                    {
                        table.AddCell(new PdfPCell(new Phrase(m.FullName, new Font(Font.FontFamily.HELVETICA, 10))));
                        table.AddCell(new PdfPCell(new Phrase(m.Email, new Font(Font.FontFamily.HELVETICA, 10))));
                        table.AddCell(new PdfPCell(new Phrase(m.PhoneNumber, new Font(Font.FontFamily.HELVETICA, 10))));
                        table.AddCell(new PdfPCell(new Phrase(m.Position, new Font(Font.FontFamily.HELVETICA, 10))));
                        table.AddCell(new PdfPCell(new Phrase(m.Department, new Font(Font.FontFamily.HELVETICA, 10))));
                        table.AddCell(new PdfPCell(new Phrase(m.ProjectName, new Font(Font.FontFamily.HELVETICA, 10))));
                    }

                    document.Add(table);
                    document.Close();
                    writer.Close();

                    byte[] pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Managers-{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error generating PDF: {ex.Message}");
                }
                finally
                {
                    memoryStream.Close();
                }
            }
        }
        #endregion

        #region Commented Code
        //public IActionResult AssignProject(ProjectViewModel projectViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("AssignProject", projectViewModel);
        //    }
        //    var employee = _employeeRepository.GetById(projectViewModel.EmployeeId);
        //    if (employee == null)
        //    {
        //        ModelState.AddModelError("Employee", "This employee does not exist");
        //        return View("AssignProject", projectViewModel);
        //    }
        //    var project = _projectRepository.GetProjectById(projectViewModel.ProjectId);
        //    if (project == null)
        //    {
        //        ModelState.AddModelError("Project", "This project does not exist");
        //        return View("AssignProject", projectViewModel);
        //    }
        //    if (employee.Projects == null)
        //    {
        //        employee.Projects = new List<Project>();
        //    }
        //    employee.Projects.Add(project);
        //    _employeeRepository.Update(employee);
        //    _employeeRepository.SaveInDB();
        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult DeleteProject(ProjectViewModel projectViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("DeleteProject", projectViewModel);
        //    }
        //    var employee = _employeeRepository.GetById(projectViewModel.EmployeeId);
        //    if (employee == null)
        //    {
        //        ModelState.AddModelError("Employee", "This employee does not exist");
        //        return View("DeleteProject", projectViewModel);
        //    }
        //    var project = _projectRepository.GetProjectById(projectViewModel.ProjectId);
        //    if (project == null)
        //    {
        //        ModelState.AddModelError("Project", "This project does not exist");
        //        return View("DeleteProject", projectViewModel);
        //    }
        //    if (employee.Projects == null)
        //    {
        //        ModelState.AddModelError("Project", "This project is not assigned to employee");
        //        return View("DeleteProject", projectViewModel);
        //    }
        //    var isProjectAssigned = employee.Projects.FirstOrDefault(p => p.ProjectId == project.ProjectId);
        //    if (isProjectAssigned == null)
        //    {
        //        ModelState.AddModelError("Project", "This project is not assigned to employee");
        //        return View("DeleteProject", projectViewModel);
        //    }
        //    employee.Projects.Remove(project);
        //    _employeeRepository.Update(employee);
        //    _employeeRepository.SaveInDB();
        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult AssignTask(TaskViewModel taskViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        taskViewModel.Tasks = _taskRepository.GetAllTasks();
        //        taskViewModel.Employees = _employeeRepository.GetAll();
        //        return View("AssignTask", taskViewModel);
        //    }
        //    var employee = _employeeRepository.GetById(taskViewModel.EmployeeId);
        //    if (employee == null)
        //    {
        //        ModelState.AddModelError("Employee", "This employee does not exist");
        //        taskViewModel.Tasks = _taskRepository.GetAllTasks();
        //        return View("AssignTask", taskViewModel);
        //    }
        //    var task = _taskRepository.GetTaskById(taskViewModel.TaskId);
        //    if (task == null)
        //    {
        //        ModelState.AddModelError("Task", "This task does not exist");
        //        taskViewModel.Tasks = _taskRepository.GetAllTasks();
        //        return View("AssignTask", taskViewModel);
        //    }
        //    var project = _projectRepository.GetProjectById(task.ProjectId);
        //    var employeeInProject = project.AssignedEmployees.FirstOrDefault(e => e.Id == employee.Id);
        //    if (employeeInProject == null)
        //    {
        //        ModelState.AddModelError("Task", "This project is not assigned to the employee");
        //        taskViewModel.Tasks = _taskRepository.GetAllTasks();
        //        return View("AssignTask", taskViewModel);
        //    }
        //    if (employee.AssignedTasks == null)
        //    {
        //        employee.AssignedTasks = new List<ProjectTask>();
        //    }
        //    employee.AssignedTasks.Add(task);
        //    _employeeRepository.Update(employee);
        //    _employeeRepository.SaveInDB();
        //    return RedirectToAction(nameof(AssignTask));
        //}

        //public IActionResult DeleteTask(TaskViewModel taskViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    var employee = _employeeRepository.GetById(taskViewModel.EmployeeId);
        //    if (employee == null)
        //    {
        //        ModelState.AddModelError("Employee", "This Employee does not exist");
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    var task = _taskRepository.GetTaskById(taskViewModel.TaskId);
        //    if (task == null)
        //    {
        //        ModelState.AddModelError("Task", "This Task does not exist");
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    var project = _projectRepository.GetProjectById(task.ProjectId);
        //    var employeeInProject = project.AssignedEmployees.FirstOrDefault(e => e.Id == employee.Id);
        //    if (employeeInProject == null)
        //    {
        //        ModelState.AddModelError("Task", "This project does not assigned to employee");
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    if (employee.AssignedTasks == null)
        //    {
        //        ModelState.AddModelError("Task", "This Task does not assigned to employee");
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    var isTaskAssigned = employee.AssignedTasks.FirstOrDefault(t => t.TaskId == task.TaskId);
        //    if (isTaskAssigned == null)
        //    {
        //        ModelState.AddModelError("Task", "This Task does not assigned to employee");
        //        return View("DeleteTask", taskViewModel);
        //    }
        //    employee.AssignedTasks.Remove(task);
        //    _employeeRepository.Update(employee);
        //    _employeeRepository.SaveInDB();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion

        #region Performance Management
        public IActionResult AddPerformance(PerformanceViewModel performanceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AddPerformance", performanceViewModel);
            }
            var employee = _employeeRepository.GetById(performanceViewModel.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("Employee", "This Employee does not exist");
                return View("AddPerformance", performanceViewModel);
            }
            var task = _taskRepository.GetTaskById(performanceViewModel.TaskId);
            if (task == null)
            {
                ModelState.AddModelError("Task", "This Task does not exist");
                return View("AddPerformance", performanceViewModel);
            }
            var performance = new Performance
            {
                EmployeeId = employee.Id,
                TaskId = task.TaskId,
                Score = performanceViewModel.Score
            };
            _performanceRepository.AddPerformance(performance);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdatePerformance(EditPerformanceViewModel editPerformanceViewModel)
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
            performance.Score = editPerformanceViewModel.Score;
            _performanceRepository.UpdatePerformance(performance);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeletePerformance(int id)
        {
            var performance = _performanceRepository.GetPerformanceById(id);
            if (performance == null)
            {
                ModelState.AddModelError("Performance", "This performance does not exist");
                return View("DeletePerformance", performance);
            }
            _performanceRepository.DeletePerformance(id);
            return View("DeletePerformance", performance);
        }
        #endregion
    }
}