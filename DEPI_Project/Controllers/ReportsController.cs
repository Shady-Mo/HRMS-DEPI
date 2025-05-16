using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Repositories.ReportsRepo;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        IReportsRepository reportsRepository;

        public ReportsController(IReportsRepository _reportsRepository)
        {
            reportsRepository = _reportsRepository;
        }

        #region Attendance
        public IActionResult AttendanceReport(string? searchName, DateTime? date)
        {
            var attendances = reportsRepository.GetAttendanceReport(searchName, date);
            return View("AttendanceReport", attendances);
        }

        public IActionResult ExportAttendanceToExcel(string? searchName, DateTime? date)
        {
            var attendances = reportsRepository.GetAttendanceReport(searchName, date);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Report");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Employee Name";
                worksheet.Cell(currentRow, 2).Value = "Date";
                worksheet.Cell(currentRow, 3).Value = "Time In";
                worksheet.Cell(currentRow, 4).Value = "Time Out";
                worksheet.Cell(currentRow, 5).Value = "Status";
                worksheet.Cell(currentRow, 6).Value = "Total Hours";

                foreach (var item in attendances)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Employee?.FirstName;
                    worksheet.Cell(currentRow, 2).Value = item.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 3).Value = item.TimeIn.ToString(@"hh\:mm");
                    worksheet.Cell(currentRow, 4).Value = item.TimeOut.ToString(@"hh\:mm");
                    worksheet.Cell(currentRow, 5).Value = item.Status.ToString();
                    worksheet.Cell(currentRow, 6).Value = item.TotalHours().ToString(@"hh\:mm");
                }

                // Style the headers
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AttendanceReport.xlsx");
                }
            }
        }
        #endregion

        #region Leave

        public IActionResult LeaveReport(string? employeeName, DateTime? startDate, DateTime? endDate, LeaveStatus? status)
        {
            var leaves = reportsRepository.GetLeaveReport(employeeName, startDate,endDate,status);
            ViewBag.StatusList = Enum.GetValues(typeof(LeaveStatus))
                            .Cast<LeaveStatus>()
                            .Select(s => new SelectListItem
                            {
                                Value = s.ToString(),
                                Text = s.ToString()
                            }).ToList();
            return View("LeaveReport", leaves);
        }

        public IActionResult ExportLeaveToExcel(string? employeeName, DateTime? startDate, DateTime? endDate, LeaveStatus? status)
        {
            var leaves = reportsRepository.GetLeaveReport(employeeName, startDate, endDate, status);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Leave Report");
                var currentRow = 1;

                // Headers
                worksheet.Cell(currentRow, 1).Value = "Employee Name";
                worksheet.Cell(currentRow, 2).Value = "Start Date";
                worksheet.Cell(currentRow, 3).Value = "End Date";
                worksheet.Cell(currentRow, 4).Value = "Reason";
                worksheet.Cell(currentRow, 5).Value = "Status";
                worksheet.Cell(currentRow, 6).Value = "Approval Date";

                // Data
                foreach (var item in leaves)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Employee?.FirstName + " " + item.Employee?.LastName;
                    worksheet.Cell(currentRow, 2).Value = item.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 3).Value = item.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 4).Value = item.Reason;
                    worksheet.Cell(currentRow, 5).Value = item.Status.ToString();
                    worksheet.Cell(currentRow, 6).Value = item.ApprovalDate?.ToString("yyyy-MM-dd") ?? "-";
                }

                // Style the headers
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "LeaveReport.xlsx");
                }
            }
        }
        #endregion

        #region Holiday

        public IActionResult HolidayReport(int? year)
        {
            var holidays = reportsRepository.GetHolidayReport(year);
            return View("HolidayReport", holidays);
        }

        public IActionResult ExportHolidayToExcel(int? year)
        {
            var holidays = reportsRepository.GetHolidayReport(year);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Holiday Report");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Holiday Name";
                worksheet.Cell(currentRow, 2).Value = "Start Date";
                worksheet.Cell(currentRow, 3).Value = "End Date";
                worksheet.Cell(currentRow, 4).Value = "Description";

                foreach (var holiday in holidays)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = holiday.HolidayName;
                    worksheet.Cell(currentRow, 2).Value = holiday.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 3).Value = holiday.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 4).Value = holiday.HolidayDuration;
                }

                // Style the headers
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"HolidayReport_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }


        #endregion




    }
}
