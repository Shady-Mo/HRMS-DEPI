using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories;
using DEPI_Project.Repositories.AttendanceRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeaveController : Controller
    {
        ILeaveRepository leaveRepository;
        CorpMgmt_SystemContext context;

        public LeaveController(ILeaveRepository _leaveRepository, CorpMgmt_SystemContext _context)
        {
            leaveRepository = _leaveRepository;
            context = _context;
        }
        public IActionResult Index()
        {
            return View("Index", leaveRepository.GetAllLeaves());
        }


        #region Add
        public IActionResult Add()
        {
            
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            return View("Add");
        }
        [HttpPost]
        public IActionResult SaveAdd(Leave leave)
        {
            
            if (leave.EndDate <= leave.StartDate)
                ModelState.AddModelError("StartDate", "The End Date Must Be After The Start Date");

            if (ModelState.IsValid && leave.EmpId != "0")
            {
                leaveRepository.AddLeave(leave);
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            
            return View("Add", leave);
        }

        #endregion

        #region Edit => Approve / Reject

        public IActionResult Approve(int id)
        {
            leaveRepository.ApproveLeave(id);
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Reject(int id)
        {
            leaveRepository.RejectLeave(id);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            var leaveFromDB = leaveRepository.GetLeaveById(id);
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");

            return View("Edit", leaveFromDB);
        }
        [HttpPost]
        public IActionResult SaveEdit(Leave leaveFromReq)
        {
            if (leaveFromReq.EndDate <= leaveFromReq.StartDate)
                ModelState.AddModelError("EndDate", "The End Date Must Be After The Start Date");

            if (ModelState.IsValid && leaveFromReq.EmpId != "0" )
            {
                leaveRepository.UpdateLeave(leaveFromReq);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.EmpList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");

            return View("Edit", leaveFromReq);
        }

        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            var leave = leaveRepository.GetLeaveById(id);
            return View("Delete", leave);
        }
        [HttpPost]
        public IActionResult SaveDelete(int id)
        {
            var leave = leaveRepository.GetLeaveById(id);

            leaveRepository.DeleteLeave(leave);
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Export Leaves to Excel
        public IActionResult ExportLeavesToExcel()
        {
            var leaves = leaveRepository.GetAllLeaves()
                .Select(l => new
                {
                    EmployeeName = l.Employee != null ? l.Employee.FirstName + " " + l.Employee.LastName : "N/A",
                    LeaveType = l.Type.ToString(),
                    Status = l.Status.ToString(),
                    StartDate = l.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = l.EndDate.ToString("yyyy-MM-dd"),
                    RequestDate = l.RequestDate.ToString("yyyy-MM-dd"),
                    ApprovalDate = l.ApprovalDate?.ToString("yyyy-MM-dd") ?? "Not Approved Yet",
                    Reason = l.Reason,
                    Duration = l.TotalLeaveDays()
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Leaves");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Employee Name";
                worksheet.Cell(currentRow, 2).Value = "Leave Type";
                worksheet.Cell(currentRow, 3).Value = "Status";
                worksheet.Cell(currentRow, 4).Value = "Start Date";
                worksheet.Cell(currentRow, 5).Value = "End Date";
                worksheet.Cell(currentRow, 6).Value = "Request Date";
                worksheet.Cell(currentRow, 7).Value = "Approval Date";
                worksheet.Cell(currentRow, 8).Value = "Reason";
                worksheet.Cell(currentRow, 9).Value = "Duration (Days)";

                // Data
                foreach (var l in leaves)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = l.EmployeeName;
                    worksheet.Cell(currentRow, 2).Value = l.LeaveType;
                    worksheet.Cell(currentRow, 3).Value = l.Status;
                    worksheet.Cell(currentRow, 4).Value = l.StartDate;
                    worksheet.Cell(currentRow, 5).Value = l.EndDate;
                    worksheet.Cell(currentRow, 6).Value = l.RequestDate;
                    worksheet.Cell(currentRow, 7).Value = l.ApprovalDate;
                    worksheet.Cell(currentRow, 8).Value = l.Reason;
                    worksheet.Cell(currentRow, 9).Value = l.Duration;
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Leaves-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion

    }
}
