using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Repositories.AttendanceRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace DEPI_Project.Controllers
{
    public class AttendanceController : Controller
    {
        IAttendanceRepository attendanceRepository;
        CorpMgmt_SystemContext context;
        public AttendanceController(IAttendanceRepository _attendanceRepository, CorpMgmt_SystemContext _context)
        {
            attendanceRepository = _attendanceRepository;
            context = _context;
        }
        public IActionResult Index()
        {
            return View("Index", attendanceRepository.GetAllAttendances());
        }

        #region Add
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            ViewBag.UsersList = new SelectList(context.Employees
                .Where(e => e.Type == "Admin")
               .ToList(), "Id", "UserName");
            ViewBag.EmpList = attendanceRepository.GetUsersAsSelectList();
            return View("Add");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveAdd(Attendance AttFromReq)
        {
            if (AttFromReq.Date > DateTime.Today)
                ModelState.AddModelError("Date", "You Can Not Record Attendance In The Future");

            if (AttFromReq.TimeOut <= AttFromReq.TimeIn)
                ModelState.AddModelError("TimeOut", "The Check-out Time Must Be After The Check-in Time");

            if (ModelState.IsValid && AttFromReq.EmpId != "0" && AttFromReq.AppUserId != "0")
            {
                attendanceRepository.AddAttendance(AttFromReq);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UsersList = new SelectList(context.Employees
                  .Where(e => e.Type == "Admin")
                 .ToList(), "Id", "UserName");
            ViewBag.EmpList = attendanceRepository.GetUsersAsSelectList();

            return View("Add", AttFromReq);
        }


        #endregion

        #region Edit 
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var AttFromDB = attendanceRepository.GetAttendanceById(id);
            ViewBag.UsersList = new SelectList(context.Employees
                 .Where(e => e.Type == "Admin")
                .ToList(), "Id", "UserName");
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");

            return View("Edit", AttFromDB);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveEdit(Attendance AttFromReq)
        {
            if (AttFromReq.Date > DateTime.Today)
                ModelState.AddModelError("Date", "You Can Not Record Attendance In The Future");

            if (AttFromReq.TimeOut <= AttFromReq.TimeIn)
                ModelState.AddModelError("TimeOut", "The Check-out Time Must Be After The Check-in Time");

            if (ModelState.IsValid)
            {
                attendanceRepository.UpdateAttendance(AttFromReq);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UsersList = new SelectList(context.Employees
                 .Where(e => e.Type == "Admin")
                .ToList(), "Id", "UserName");
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "FirstName");

            return View("Edit", AttFromReq);

        }

        #endregion

        #region Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var att = attendanceRepository.GetAttendanceById(id);
            return View("Delete", att);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveDelete(int id)
        {
            var att = attendanceRepository.GetAttendanceById(id);

            attendanceRepository.DeleteAttendance(att);
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Export Attendance to Excel
        [Authorize(Roles = "Admin")]
        public IActionResult ExportAttendanceToExcel()
        {
            var attendances = attendanceRepository.GetAllAttendances()
                .Select(a => new
                {
                    EmployeeName = a.Employee != null ? a.Employee.FirstName + " " + a.Employee.LastName : "N/A",
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    TimeIn = a.TimeIn.ToString(@"hh\:mm"),
                    TimeOut = a.TimeOut.ToString(@"hh\:mm"),
                    Status = a.Status.ToString(),
                    TotalHours = a.TotalHours().ToString(@"hh\:mm")
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Employee Name";
                worksheet.Cell(currentRow, 2).Value = "Date";
                worksheet.Cell(currentRow, 3).Value = "Time In";
                worksheet.Cell(currentRow, 4).Value = "Time Out";
                worksheet.Cell(currentRow, 5).Value = "Status";
                worksheet.Cell(currentRow, 6).Value = "Total Hours";

                // Data
                foreach (var a in attendances)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = a.EmployeeName;
                    worksheet.Cell(currentRow, 2).Value = a.Date;
                    worksheet.Cell(currentRow, 3).Value = a.TimeIn;
                    worksheet.Cell(currentRow, 4).Value = a.TimeOut;
                    worksheet.Cell(currentRow, 5).Value = a.Status;
                    worksheet.Cell(currentRow, 6).Value = a.TotalHours;
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
                        $"Attendance-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion




    }
}
