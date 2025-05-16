using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Repositories;
using DEPI_Project.Repositories.JobRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JobController : Controller
    {
        IJobRepository _jobRepository;
        IDepartmentRepository _departmentRepository;
        IEmployeeRepository _employeeRepository;
        public JobController(IJobRepository jobRepository , IDepartmentRepository departmentRepository , IEmployeeRepository employeeRepository  )
        {
            _jobRepository = jobRepository;
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;

        }

        #region GET All
        
        public ActionResult Index()
        {
            ViewData["DeptList"] =_departmentRepository.GetAllDepartments();
            List<Job> JobModel = _jobRepository.GetAll();
            return View(JobModel);
        }
        #endregion

        #region Details
        public ActionResult Details(int id)
        {
            ViewData["DeptList"] = _departmentRepository.GetAllDepartments();
            Job Job = _jobRepository.GetById(id);
            return View(Job);
        }
        #endregion

        #region Add
        public ActionResult Add()
        {
            ViewData["UserList"] = _employeeRepository.GetAdminList();
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            return View();
        }
        #endregion

        #region SaveAdd
        [HttpPost]
        public IActionResult SaveAdd(Job JobFromReq)
        {

            if (ModelState.IsValid)
            {
                _jobRepository.Add(JobFromReq);
                _jobRepository.SaveInDB();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserList"] = _employeeRepository.GetAdminList();
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            TempData["Success"] = "Job saved successfully";
            return View("Add", JobFromReq);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            ViewData["UserList"] = _employeeRepository.GetAdminList();
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            Job Job = _jobRepository.GetById(id);
            return View(Job);
        }
        #endregion

        #region SaveEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEdit(Job JobFromReq, [FromRoute] int id)
        {
            if (id != JobFromReq.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                Job JobFromDB = _jobRepository.GetById(id);
                JobFromDB.AppUserId=JobFromReq.AppUserId;
                JobFromDB.Name = JobFromReq.Name;
                JobFromDB.Role= JobFromReq.Role;
                JobFromDB.Vacancies = JobFromReq.Vacancies;
                JobFromDB.ExpireDate = JobFromReq.ExpireDate;
                JobFromDB.DepartmentId = JobFromReq.DepartmentId;
                JobFromDB.Status = JobFromReq.Status; 
                JobFromDB.JobType = JobFromReq.JobType;
                _jobRepository.SaveInDB();
                TempData["EditSuccess"] = "Job updated successfully";

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserList"] = _employeeRepository.GetAdminList();
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            return View("Edit", JobFromReq);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {
            Job job = _jobRepository.GetById(id);
            if (ModelState.IsValid)
            {
                _jobRepository.Delete(job);
                _jobRepository.SaveInDB();
                TempData["DeleteSuccess"] = "Job deleted successfully";
                return RedirectToAction("Index");
            }
            return NotFound();

        }
        #endregion

        #region Export to Excel
        public IActionResult ExportToExcel()
        {
            var jobs = _jobRepository.GetAll()
                .Select(j => new
                {
                    JobTitle = j.Name,
                    Status = j.Status.ToString(),
                    DatePosted = j.ExpireDate.ToString("yyyy-MM-dd"),
                    Role = j.Role,
                    Vacancies = j.Vacancies,
                    Department = j.Department?.Name ?? "N/A",
                    JobType = j.JobType.ToString()
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Jobs");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Job Title";
                worksheet.Cell(currentRow, 2).Value = "Status";
                worksheet.Cell(currentRow, 3).Value = "Date Posted";
                worksheet.Cell(currentRow, 4).Value = "Role";
                worksheet.Cell(currentRow, 5).Value = "Vacancies";
                worksheet.Cell(currentRow, 6).Value = "Department";
                worksheet.Cell(currentRow, 7).Value = "Job Type";

                // Data Rows
                foreach (var job in jobs)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = job.JobTitle;
                    worksheet.Cell(currentRow, 2).Value = job.Status;
                    worksheet.Cell(currentRow, 3).Value = job.DatePosted;
                    worksheet.Cell(currentRow, 4).Value = job.Role;
                    worksheet.Cell(currentRow, 5).Value = job.Vacancies;
                    worksheet.Cell(currentRow, 6).Value = job.Department;
                    worksheet.Cell(currentRow, 7).Value = job.JobType;
                }

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                // Auto fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Jobs-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }

        #endregion
    }
}
