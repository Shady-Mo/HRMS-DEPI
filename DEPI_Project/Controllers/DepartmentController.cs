using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.View_Model;
using DEPI_Project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly CorpMgmt_SystemContext _context;

        public DepartmentController(IDepartmentRepository departmentRepository, CorpMgmt_SystemContext context)
        {
            _departmentRepository = departmentRepository;
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var departments = _departmentRepository.GetAllDepartments().AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments.Where(d =>
                    d.Name.Contains(searchString) ||
                    d.Manager.FirstName.Contains(searchString));
            }

            return View("Index", departments.ToList());
        }

        #region Add
        public IActionResult Add()
        {
            var validManagers = _context.Employees
                .Where(e => e.Type == "Manager")
                .Select(m => m.ManagedDepartment == null)
                .ToList();
            ViewBag.ManagersList = new SelectList(validManagers, "Id", "UserName");
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(DepartmentViewModel deptFromReq)
        {
            if (deptFromReq.EstablishedYear > DateTime.Today.Year) {
                ModelState.AddModelError("EstablishedYear", "Established Year cannot be in the future");
                return View("Add", new Department());
            }

            if (ModelState.IsValid)
            {
                var department = new Department() {
                    Name = deptFromReq.Name,
                    EmployeeCapacity = deptFromReq.EmployeeCapacity,
                    EstablishedYear = deptFromReq.EstablishedYear,
                    Location = deptFromReq.Location
                };
                _departmentRepository.AddDepartment(department);
                return RedirectToAction(nameof(Index));
            }

            return View("Add", new Department());
        }
        #endregion

        #region Edit
        public IActionResult Edit(int id)
        {
            var deptFromDB = _departmentRepository.GetDepartmentById(id);
            if (deptFromDB == null) return NotFound();

            var department = _context.Departments
                .FirstOrDefault(d => d.DepartmentId == id);

            var manager = _context.Employees
                .FirstOrDefault(m => m.Id == department.ManagerId);

            if (manager != null) {
                var validManagers = _context.Employees
                    .Where(e => e.Type == "Manager" && 
                        (e.Id == manager.Id || _context.Departments.All(d => d.ManagerId != e.Id)))
                    .ToList();
                ViewBag.ManagersList = new SelectList(validManagers, "Id", "UserName");
                return View("Edit", deptFromDB);
            }

            else {
                var validManagers = _context.Employees
                    .Where(e => e.Type == "Manager" && 
                        _context.Departments.All(d => d.ManagerId != e.Id))
                    .ToList();
                ViewBag.ManagersList = new SelectList(validManagers, "Id", "UserName");
                return View("Edit", deptFromDB);
            }
        }

        [HttpPost]
        public IActionResult SaveEdit(Department deptFromReq)
        {
            if (deptFromReq.EmployeeCapacity < (deptFromReq.Employees?.Count ?? 0))
                ModelState.AddModelError("EmployeeCapacity", "Employee Capacity cannot be less than current employee count");

            if (deptFromReq.EstablishedYear > DateTime.Today.Year)
                ModelState.AddModelError("EstablishedYear", "Established Year cannot be in the future");

            if (ModelState.IsValid)
            {
                _departmentRepository.UpdateDepartment(deptFromReq);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ManagersList = new SelectList(_context.Employees.ToList(), "Id", "FirstName");
            return View("Edit", deptFromReq);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            var dept = _departmentRepository.GetDepartmentById(id);
            if (dept == null) return NotFound();

            return View("Delete", dept);
        }

        [HttpPost]
        public IActionResult SaveDelete(int id)
        {
            var dept = _departmentRepository.GetDepartmentById(id);
            if (dept == null) return NotFound();

            _departmentRepository.DeleteDepartment(dept);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Export
        public IActionResult Export()
        {
            // Placeholder for export logic
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Get Manager Details
        [HttpGet]
        public IActionResult GetManagerDetails(string id)
        {
            var manager = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (manager == null)
            {
                return Json(new { phoneNumber = "", email = "" });
            }

            return Json(new { phoneNumber = manager.PhoneNumber, email = manager.Email });
        }
        #endregion
    }
}