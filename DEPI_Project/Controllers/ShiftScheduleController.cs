using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace DEPI_Project.Controllers {
    [Authorize(Roles = "Admin")]
    public class ShiftScheduleController: Controller {
        private readonly IShiftScheduleRepository shiftScheduleRepository;
        private readonly CorpMgmt_SystemContext context;

        public ShiftScheduleController(IShiftScheduleRepository _shiftScheduleRepository, CorpMgmt_SystemContext _context) {
            shiftScheduleRepository = _shiftScheduleRepository;
            context = _context;
        }

        public IActionResult Index() {
            return View("Index", shiftScheduleRepository.GetAllShifts());
        }

        #region Add
        public IActionResult Add() {
            ViewBag.EmployeeList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            ViewBag.ManagerList = new SelectList(context.Employees.Where(e => e.Type == "Manager").ToList(), "Id", "UserName");
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(ShiftSchedule shift) {
            if (shift.EndTime <= shift.StartTime)
                ModelState.AddModelError("EndTime", "The End Time must be after the Start Time");

            if (shift.BreakEnd <= shift.BreakStart)
                ModelState.AddModelError("BreakEnd", "The Break End Time must be after the Break Start Time");

            if (shift.EmployeeId != "0" && shift.ManagerId != "0") {
                shift.Status = ShiftStatus.Pending;
                shiftScheduleRepository.AddShift(shift);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            ViewBag.ManagerList = new SelectList(context.Employees.Where(e => e.Type == "Manager").ToList(), "Id", "UserName");
            return View("Add", shift);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int id) {
            var shiftFromDB = shiftScheduleRepository.GetShiftById(id);
            ViewBag.EmployeeList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            ViewBag.ManagerList = new SelectList(context.Employees.Where(e => e.Type == "Manager").ToList(), "Id", "UserName");
            return View("Edit", shiftFromDB);
        }

        [HttpPost]
        public IActionResult SaveEdit(ShiftSchedule shiftFromReq) {
            if (shiftFromReq.EndTime <= shiftFromReq.StartTime)
                ModelState.AddModelError("EndTime", "The End Time must be after the Start Time");

            if (shiftFromReq.BreakEnd <= shiftFromReq.BreakStart)
                ModelState.AddModelError("BreakEnd", "The Break End Time must be after the Break Start Time");

            if (shiftFromReq.EmployeeId != "0" && shiftFromReq.ManagerId != "0") {
                shiftScheduleRepository.UpdateShift(shiftFromReq);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeList = new SelectList(context.Employees.Where(e => e.Type == "Employee").ToList(), "Id", "UserName");
            ViewBag.ManagerList = new SelectList(context.Employees.Where(e => e.Type == "Manager").ToList(), "Id", "UserName");
            return View("Edit", shiftFromReq);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id) {
            var shift = shiftScheduleRepository.GetShiftById(id);
            return View("Delete", shift);
        }

        [HttpPost]
        public IActionResult SaveDelete(int id) {
            var shift = shiftScheduleRepository.GetShiftById(id);
            shiftScheduleRepository.DeleteShift(shift);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Mark Complete / Cancel
        public IActionResult MarkComplete(int id) {
            shiftScheduleRepository.MarkShiftCompleted(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cancel(int id) {
            shiftScheduleRepository.CancelShift(id);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Export to Excel
        public IActionResult ExportToExcel() {
            var shifts = shiftScheduleRepository.GetAllShifts()
                .Select(s => new {
                    EmployeeName = s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : "N/A",
                    ManagerName = s.Manager != null ? s.Manager.FirstName + " " + s.Manager.LastName : "N/A",
                    ShiftDate = s.ShiftDate.ToString("yyyy-MM-dd"),
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm"),
                    TotalHours = s.TotalHours().ToString("F2"),
                    ShiftType = s.ShiftType?.ToString() ?? "N/A",
                    Status = s.Status?.ToString() ?? "N/A"
                })
                .ToList();

            using (var workbook = new XLWorkbook()) {
                var worksheet = workbook.Worksheets.Add("ShiftSchedules");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Employee Name";
                worksheet.Cell(currentRow, 2).Value = "Manager Name";
                worksheet.Cell(currentRow, 3).Value = "Shift Date";
                worksheet.Cell(currentRow, 4).Value = "Start Time";
                worksheet.Cell(currentRow, 5).Value = "End Time";
                worksheet.Cell(currentRow, 6).Value = "Total Hours";
                worksheet.Cell(currentRow, 7).Value = "Shift Type";
                worksheet.Cell(currentRow, 8).Value = "Status";

                // Data
                foreach (var s in shifts) {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = s.EmployeeName;
                    worksheet.Cell(currentRow, 2).Value = s.ManagerName;
                    worksheet.Cell(currentRow, 3).Value = s.ShiftDate;
                    worksheet.Cell(currentRow, 4).Value = s.StartTime;
                    worksheet.Cell(currentRow, 5).Value = s.EndTime;
                    worksheet.Cell(currentRow, 6).Value = s.TotalHours;
                    worksheet.Cell(currentRow, 7).Value = s.ShiftType;
                    worksheet.Cell(currentRow, 8).Value = s.Status;
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream()) {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ShiftSchedules-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion
    }
}