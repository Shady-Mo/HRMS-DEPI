using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace DEPI_Project.Controllers
{
    public class HolidayController : Controller
    {
        IHolidayRepository holidayRepository;
        CorpMgmt_SystemContext context;

        public HolidayController(IHolidayRepository _holidayRepository, CorpMgmt_SystemContext _context)
        {
            holidayRepository = _holidayRepository;
            context = _context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View("Index", holidayRepository.GetAllHolidays());
        }
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult IndexEmp()
        {
            return View("IndexEmp", holidayRepository.GetAllHolidays());
        }

        #region Add
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            ViewBag.UsersList = new SelectList(context.Employees
                .Where(e => e.Type == "Admin")
               . ToList(), "Id", "UserName");

            return View("Add");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveAdd(Holiday holidayFromReq)
        {
            if (holidayFromReq.StartDate < DateTime.Today)
                ModelState.AddModelError("StartDate", "The Start Date Of The Vacation Cannot Be In The Past");

            if (holidayFromReq.EndDate < holidayFromReq.StartDate)
                ModelState.AddModelError("EndDate", "The End Date Of The Vacation Must Be After The Start Date");
            if (holidayFromReq.UserId == "0")
            {
                ModelState.AddModelError("UserId", "Please select a user.");
            }

            if (ModelState.IsValid)
            {
                holidayRepository.AddHoliday(holidayFromReq);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UsersList = new SelectList(context.Employees
                .Where(e => e.Type == "Admin")
               .ToList(), "Id", "UserName");
            return View("Add", holidayFromReq);
        }

        #endregion

        #region Edit 
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Holiday holidayFromDB = holidayRepository.GetHolidayById(id);
            ViewBag.UsersList = new SelectList(context.Employees
                 .Where(e => e.Type == "Admin")
                .ToList(), "Id", "UserName");

            return View("Edit", holidayFromDB);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveEdit(Holiday holidayFromReq)
        {
            if (holidayFromReq.StartDate < DateTime.Today)
                ModelState.AddModelError("StartDate", "The Start Date Of The Holiday Cannot Be In The Past");

            if (holidayFromReq.EndDate < holidayFromReq.StartDate)
                ModelState.AddModelError("EndDate", "The End Date Of The Holiday Must Be After The Start Date");

            if (ModelState.IsValid && holidayFromReq.UserId != "0")
            {
                holidayRepository.UpdateHoliday(holidayFromReq);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UsersList = new SelectList(context.Employees
                 .Where(e => e.Type == "Admin")
                .ToList(), "Id", "UserName");
            return View("Edit", holidayFromReq);

        }

        #endregion

        #region Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var holiday = holidayRepository.GetHolidayById(id);
            return View("Delete", holiday);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveDelete(int id)
        {
            var holiday = holidayRepository.GetHolidayById(id);

            holidayRepository.DeleteHoliday(holiday);
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Export Holidays to Excel
        [Authorize(Roles = "Admin")]
        public IActionResult ExportHolidaysToExcel()
        {
            var holidays = holidayRepository.GetAllHolidays()
                .Select(h => new
                {
                    HolidayName = h.HolidayName,
                    StartDate = h.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = h.EndDate.ToString("yyyy-MM-dd"),
                    Duration = h.HolidayDuration,
                    EmployeeName = h.AppUser != null ? h.AppUser.FirstName + " " + h.AppUser.LastName : "N/A"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Holidays");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Holiday Name";
                worksheet.Cell(currentRow, 2).Value = "Start Date";
                worksheet.Cell(currentRow, 3).Value = "End Date";
                worksheet.Cell(currentRow, 4).Value = "Duration (Days)";
                worksheet.Cell(currentRow, 5).Value = "Employee Name";

                // Data
                foreach (var h in holidays)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = h.HolidayName;
                    worksheet.Cell(currentRow, 2).Value = h.StartDate;
                    worksheet.Cell(currentRow, 3).Value = h.EndDate;
                    worksheet.Cell(currentRow, 4).Value = h.Duration;
                    worksheet.Cell(currentRow, 5).Value = h.EmployeeName;
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Holidays-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion

    }
}
