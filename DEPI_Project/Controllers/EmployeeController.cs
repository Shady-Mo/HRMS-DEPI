using System.Threading.Tasks;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using DocumentFormat.OpenXml.InkML;
using ClosedXML.Excel;
using DEPI_Project.Models.View_Model;
using DEPI_Project.DTOs;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace DEPI_Project.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        IEmployeeRepository _employeeRepository;
        IDepartmentRepository _departmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //CorpMgmt_SystemContext dbcontext;
        public EmployeeController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            //dbcontext = new CorpMgmt_SystemContext();
        }
        #region GetALL Emp
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            List<Employee> employees = _employeeRepository.GetAll();
            return View(employees);
        }
        #endregion

        #region Details
        [Authorize(Roles = "Admin")]
        public IActionResult Details(string id)
        {
            ViewData["DeptList"] = _departmentRepository.GetAllDepartments();
            Employee Employee = _employeeRepository.GetById(id);
            return View(Employee);
        }
        #endregion

        #region Add
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            return View();
        }
        #endregion

        #region Save Add
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveAdd(EmployeeViewModel EmpFromReq)
        {

            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(EmpFromReq.Email);
                if (userExists != null)
                {
                    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                    ModelState.AddModelError("Email", "Email already exists");
                    return View("Add", EmpFromReq);
                }

                var userNameExists = await _userManager.FindByNameAsync(EmpFromReq.UserName);
                if (userNameExists != null)
                {
                    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                    ModelState.AddModelError("UserName", "Username already exists");
                    return View("Add", EmpFromReq);
                }
                var employee = new Employee()
                {
                    FirstName = EmpFromReq.FirstName?.Trim(),
                    LastName = EmpFromReq.LastName?.Trim(),
                    Address = EmpFromReq.Address?.Trim(),
                    Nationality = EmpFromReq.Nationality?.Trim(),
                    PhoneNumber = EmpFromReq.PhoneNumber?.Trim(),
                    Email = EmpFromReq.Email?.Trim(),
                    Position = EmpFromReq.Position?.Trim(),
                    Skills = EmpFromReq.Skills,
                    HireDate = EmpFromReq.HireDate,
                    Salary = EmpFromReq.Salary,
                    DeptId = EmpFromReq.DeptId,
                    EmpStatus = EmpFromReq.EmpStatus,
                    MaritalStatus = EmpFromReq.MaritalStatus,
                    Gender = EmpFromReq.Gender,
                    ContractType = EmpFromReq.ContractType,
                    Type = "Employee",
                    EmailConfirmed = true,
                    UserName = EmpFromReq.UserName?.Trim(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(employee, EmpFromReq.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                    return View("Add", EmpFromReq);
                }

                var accountService = new AccountService();
                var account = await accountService.CreateAsync(new AccountCreateOptions {
                    Type = "express",
                    Country = "US",
                    Email = employee.Email,
                    Capabilities = new AccountCapabilitiesOptions {
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                    },
                    BusinessType = "individual",
                    BusinessProfile = new AccountBusinessProfileOptions {
                        ProductDescription = "Salary payment for employee"
                    },
                    Individual = new AccountIndividualOptions {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        Address = new AddressOptions {
                            Line1 = "123 Main Street",
                            City = "California",
                            PostalCode = "90002",
                            Country = "US"
                        },
                        Dob = new DobOptions {
                            Day = 1,
                            Month = 1,
                            Year = 1990
                        }
                    }
                });

                employee.StripeAccountId = account.Id;

                await _userManager.AddToRoleAsync(employee, "Employee");
                TempData["Success"] = "Employee saved successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            return View("Add", EmpFromReq);

        }
        #endregion

        #region Edit
        [Authorize(Roles = "Admin")]
        //edit
        public IActionResult Edit(string id)
        {
            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            Employee employee = _employeeRepository.GetById(id);
            if (employee == null)
                return NotFound();

            var employeeViewModel = new EmployeeViewModel
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                UserName = employee.UserName,
                Password = employee.PasswordHash,
                Email = employee.Email,
                Address = employee.Address,
                Gender = employee.Gender,
                PhoneNumber = employee.PhoneNumber,
                Nationality = employee.Nationality,
                Position = employee.Position,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                DeptId = employee.DeptId,
                EmpStatus = employee.EmpStatus,
                ContractType = employee.ContractType,
                MaritalStatus = employee.MaritalStatus,
                Skills = employee.Skills,
                Type = employee.Type,
            };
            ViewBag.EmployeeId = id;
            return View(employeeViewModel);
        }
        #endregion

        #region Save Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveEdit(EmployeeViewModel EmpFromReq, [FromForm] string id)
        {
            var employee = await _userManager.FindByIdAsync(id);
            if (employee == null)
            {
                ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                ViewBag.EmployeeId = id;
                return View("Edit", EmpFromReq);
            }
            if (ModelState.IsValid)
            {
                Employee employeeFromDB = _employeeRepository.GetById(id);

                if (employeeFromDB.Email != EmpFromReq.Email) {
                    var userExists = await _userManager.FindByEmailAsync(EmpFromReq.Email);
                    if (userExists != null) {
                        ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                        ViewBag.EmployeeId = id;
                        ModelState.AddModelError("Email", "Email already exists");
                        return View("Edit", EmpFromReq);
                    }
                }

                if (employeeFromDB.UserName != EmpFromReq.UserName) {
                    var userNameExists = await _userManager.FindByNameAsync(EmpFromReq.UserName);
                    if (userNameExists != null) {
                        ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                        ViewBag.EmployeeId = id;
                        ModelState.AddModelError("UserName", "Username already exists");
                        return View("Edit", EmpFromReq);
                    }
                }

                await _userManager.RemoveFromRoleAsync(employeeFromDB, employeeFromDB.Type);

                employeeFromDB.FirstName = EmpFromReq.FirstName?.Trim();
                employeeFromDB.LastName = EmpFromReq.LastName?.Trim();
                employeeFromDB.UserName = EmpFromReq.UserName?.Trim();
                employeeFromDB.Email = EmpFromReq.Email?.Trim();
                employeeFromDB.Address = EmpFromReq.Address?.Trim();
                employeeFromDB.Gender = EmpFromReq.Gender;
                employeeFromDB.PhoneNumber = EmpFromReq.PhoneNumber?.Trim();
                employeeFromDB.Nationality = EmpFromReq.Nationality?.Trim();
                employeeFromDB.Position = EmpFromReq.Position?.Trim();
                employeeFromDB.HireDate = EmpFromReq.HireDate;
                employeeFromDB.Salary = EmpFromReq.Salary;
                employeeFromDB.DeptId = EmpFromReq.DeptId;
                employeeFromDB.EmpStatus = EmpFromReq.EmpStatus;
                employeeFromDB.ContractType = EmpFromReq.ContractType;
                employeeFromDB.MaritalStatus = EmpFromReq.MaritalStatus;
                employeeFromDB.Skills = EmpFromReq.Skills;
                employeeFromDB.Type = EmpFromReq.Type;

                await _userManager.AddToRoleAsync(employeeFromDB, employeeFromDB.Type);
                var result = await _userManager.UpdateAsync(employeeFromDB);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
                    ViewBag.EmployeeId = id;
                    return View("Edit", EmpFromReq);
                }
                TempData["EditSuccess"] = "Employee updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewData["DeptList"] = _departmentRepository.GetDepartmentsAsSelectList();
            ViewBag.EmployeeId = id;
            return View("Edit", EmpFromReq);

        }
        #endregion

        #region Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            Employee employee = _employeeRepository.GetById(id);
            if (ModelState.IsValid)
            {
                _employeeRepository.Delete(employee);
                _employeeRepository.SaveInDB();
                TempData["DeleteSuccess"] = "Employee deleted successfully";
                return RedirectToAction("Index");
            }
            return NotFound();

        }
        #endregion

        #region Export to Excel
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToExcel()
        {
            var employees = _employeeRepository.GetAll()
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    e.Email,
                    e.PhoneNumber,
                    e.Position,
                    HireDate = e.HireDate.ToString("yyyy-MM-dd"),
                    Salary = e.Salary.ToString("C"),
                    e.EmpStatus,
                    e.ContractType,
                    e.MaritalStatus
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Full Name";
                worksheet.Cell(currentRow, 2).Value = "Email";
                worksheet.Cell(currentRow, 3).Value = "Phone";
                worksheet.Cell(currentRow, 4).Value = "Position";
                worksheet.Cell(currentRow, 5).Value = "Hire Date";
                worksheet.Cell(currentRow, 6).Value = "Salary";
                worksheet.Cell(currentRow, 7).Value = "Status";
                worksheet.Cell(currentRow, 8).Value = "Contract Type";
                worksheet.Cell(currentRow, 9).Value = "Marital Status";

                // Data
                foreach (var e in employees)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = e.FullName;
                    worksheet.Cell(currentRow, 2).Value = e.Email;
                    worksheet.Cell(currentRow, 3).Value = e.PhoneNumber;
                    worksheet.Cell(currentRow, 4).Value = e.Position;
                    worksheet.Cell(currentRow, 5).Value = e.HireDate;
                    worksheet.Cell(currentRow, 6).Value = e.Salary;
                    worksheet.Cell(currentRow, 7).Value = e.EmpStatus.ToString();
                    worksheet.Cell(currentRow, 8).Value = e.ContractType.ToString();
                    worksheet.Cell(currentRow, 9).Value = e.MaritalStatus.ToString();
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 9);
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
                        $"Employees-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
        #endregion


        //************* Emp dashboard *************
        #region Attendance
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult EmpAttendance()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            var attendances = _employeeRepository.GetMyAttendances(userId);
            var todayAttendance = _employeeRepository.GetTodayAttendance(userId);

            ViewBag.HasCheckedIn = todayAttendance != null;
            ViewBag.HasCheckedOut = todayAttendance?.TimeOut != TimeSpan.Zero;

            return View(attendances);
        }

        [HttpPost]
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult CheckIn()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userId))
            //    return Unauthorized();

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;


            var existing = _employeeRepository.GetTodayAttendance(userId);


            var attendance = new Attendance
            {
                AppUserId = userId,
                EmpId = userId,
                Date = DateTime.Today,
                TimeIn = DateTime.Now.TimeOfDay,
                Status = Status.Present
            };

            _employeeRepository.AddAttendance(attendance);

            TempData["Message"] = "Check-in successful!";
            return RedirectToAction("EmpAttendance");
        }

        [HttpPost]
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult CheckOut()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userId))
            //    return Unauthorized();

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;


            var attendance = _employeeRepository.GetTodayAttendance(userId);

            attendance.TimeOut = DateTime.Now.TimeOfDay;
            _employeeRepository.UpdateAttendance(attendance);
            TempData["Message"] = "Check-out successful!";

            return RedirectToAction("EmpAttendance");
        }

        #endregion

        #region Profile
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult Profile()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            var employee = _employeeRepository.GetEmployeeProfile(userId);

            if (employee == null)
                return NotFound("Profile not found.");

            return View(employee);
        }

        #endregion

        #region Leave
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult EmpLeave()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            var leaves = _employeeRepository.GetMyLeaves(userId);
            return View(leaves);
        }
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult AddLeave()
        {
            return View("AddLeave");
        }

        [HttpPost]
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult SaveAddedLeave(Leave leave)
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            leave.EmpId = userId;


            if (leave.EndDate <= leave.StartDate)
                ModelState.AddModelError("EndDate", "End date must be after start date");

            if (ModelState.IsValid)
            {
                _employeeRepository.SubmitLeave(leave);
                return RedirectToAction("EmpLeave");
            }

            return View("AddLeave", leave);
        }
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult Cancel(int id)
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var userId = _userManager.Users.FirstOrDefault(u => u.UserName == userName)?.Id;
            _employeeRepository.CancelLeave(id, userId);
            return RedirectToAction("EmpLeave");
        }

        #endregion
       
        #region Admin
        public IActionResult ALLAdmin()
        {
            var admin = _employeeRepository.GetAdmins();
            return View("ALLAdmin", admin);
        }
        #endregion

    }
}